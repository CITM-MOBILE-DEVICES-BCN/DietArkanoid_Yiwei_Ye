using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float speedIncreaseRate = 0.1f;

    private Rigidbody2D rb;
    private Vector2 startPosition;
    private bool isLaunched = false;
    private float currentSpeed;

    private Brick GetBrickComponent(GameObject gameObject)
    {
        Brick brick = gameObject.GetComponent<Brick>();
        if (brick == null)
        {
            brick = gameObject.GetComponentInParent<Brick>();
        }
        return brick;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = transform.position;
        currentSpeed = initialSpeed;
    }

    private void Start()
    {
        ResetBall();
    }

    private void Update()
    {
        if (!isLaunched)
        {
            // Attach the ball to the paddle before launch
            Vector3 paddlePosition = GameObject.FindGameObjectWithTag("Paddle").transform.position;
            transform.position = new Vector3(paddlePosition.x, startPosition.y, 0f);

            // Launch the ball after 2 seconds
            if (Time.timeSinceLevelLoad > 2f)
            {
                LaunchBall();
            }
        }

        // Update GameManager for auto-play
        GameManager.Instance.UpdateAutoPlay(transform.position);

        // Check if ball is below the bottom boundary
        if (BoundaryManager.Instance.IsBelowBottomBoundary(transform.position))
        {
            Debug.Log("Ball is below bottom boundary");
            GameManager.Instance.LoseLife();
            ResetBall();
        }
    }

    private void FixedUpdate()
    {
        if (isLaunched)
        {
            // Ensure constant speed
            rb.velocity = rb.velocity.normalized * currentSpeed;
        }
    }

    private void LaunchBall()
    {
        isLaunched = true;
        float randomDirection = Random.Range(-1f, 1f);
        Vector2 direction = new Vector2(randomDirection, 1f).normalized;
        rb.velocity = direction * currentSpeed;
        Debug.Log("Ball launched with velocity: " + rb.velocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name);
        if (isLaunched)
        {
            // Increase speed after each collision
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate, maxSpeed);

            Debug.Log($"Collision details - Name: {collision.gameObject.name}, Tag: {collision.gameObject.tag}, Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");

            // Check if the collision is with a brick
            Brick brick = GetBrickComponent(collision.gameObject);

            if (brick != null)
            {
                Debug.Log($"Hit brick: {brick.name}, Type: {brick.GetType().Name}");
                brick.Hit();

                // Changes: Check if the brick should be destroyed after hitting it
                if (brick.ShouldBeDestroyed())
                {
                    Debug.Log($"Destroying brick: {brick.name}");
                    Destroy(brick.gameObject);
                }
            }
            else
            {
                Debug.Log($"Collided object is not a brick. Components on this object:");
                Component[] components = collision.gameObject.GetComponents<Component>();
                foreach (Component component in components)
                {
                    Debug.Log($"- {component.GetType().Name}");
                }
            }

            // Normal collision behavior (bouncing)
            Vector2 incomingVelocity = rb.velocity;
            Vector2 normal = collision.contacts[0].normal;
            Vector2 newDirection = Vector2.Reflect(incomingVelocity, normal).normalized;
            rb.velocity = newDirection * currentSpeed;

            Debug.Log($"Ball velocity after collision: {rb.velocity}");
        }
    }

    public void ResetBall()
    {
        isLaunched = false;
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        currentSpeed = initialSpeed;
        Debug.Log("Ball reset to position: " + transform.position);
    }
}
