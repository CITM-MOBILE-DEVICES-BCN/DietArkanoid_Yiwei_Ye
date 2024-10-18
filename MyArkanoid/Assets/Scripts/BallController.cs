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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isLaunched)
        {
            // Increase speed after each collision
            currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate, maxSpeed);

            // Check if the ball hit the bottom boundary
            if (BoundaryManager.Instance.IsBottomBoundary(collision))
            {
                GameManager.Instance.LoseLife();
                ResetBall();
            }
            else
            {
                // Normal collision behavior (bouncing)
                Vector2 incomingVelocity = rb.velocity;
                Vector2 normal = collision.contacts[0].normal;
                Vector2 newDirection = Vector2.Reflect(incomingVelocity, normal).normalized;
                rb.velocity = newDirection * currentSpeed;
            }
        }
    }

    public void ResetBall()
    {
        isLaunched = false;
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        currentSpeed = initialSpeed;
    }
}