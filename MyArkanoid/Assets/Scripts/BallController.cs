using UnityEngine;

public class BallController : MonoBehaviour
{
    public float initialSpeed = 5f;
    public float maxSpeed = 15f;
    public float speedIncreaseRate = 0.1f;
    public float maxBounceAngle = 75f;
    public float minVerticalVelocity = 0.2f;
    public float randomBounceAngle = 10f;

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

        // Check if ball is below the bottom boundary
        if (transform.position.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y)
        {
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
        float randomDirection = Random.Range(-0.5f, 0.5f);
        Vector2 direction = new Vector2(randomDirection, 1f).normalized;
        rb.velocity = direction * currentSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isLaunched) return;

        // Increase speed after each collision
        currentSpeed = Mathf.Min(currentSpeed + speedIncreaseRate, maxSpeed);

        Vector2 incomingVelocity = rb.velocity;
        Vector2 normal = collision.contacts[0].normal;

        // Check if the collision is with the paddle
        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleCollision(collision);
        }
        else
        {
            HandleBoundaryAndBrickCollision(normal);
        }

        // Check if the collision is with a brick
        Brick brick = GetBrickComponent(collision.gameObject);
        if (brick != null)
        {
            brick.Hit();
            if (brick.ShouldBeDestroyed())
            {
                Destroy(brick.gameObject);
            }
        }
    }

    private void HandlePaddleCollision(Collision2D collision)
    {
        float paddleWidth = collision.transform.localScale.x;
        float hitPosition = (transform.position.x - collision.transform.position.x) / (paddleWidth / 2);

        float bounceAngle = hitPosition * maxBounceAngle;
        Vector2 newDirection = Quaternion.Euler(0, 0, bounceAngle) * Vector2.up;

        rb.velocity = newDirection * currentSpeed;
    }

    private void HandleBoundaryAndBrickCollision(Vector2 normal)
    {
        Vector2 newDirection = Vector2.Reflect(rb.velocity, normal);

        // Add a small random angle to prevent repetitive patterns
        float randomAngle = Random.Range(-randomBounceAngle, randomBounceAngle);
        newDirection = Quaternion.Euler(0, 0, randomAngle) * newDirection;

        // Ensure the ball doesn't move too horizontally
        if (Mathf.Abs(newDirection.y) < minVerticalVelocity)
        {
            newDirection.y = newDirection.y < 0 ? -minVerticalVelocity : minVerticalVelocity;
        }

        rb.velocity = newDirection.normalized * currentSpeed;
    }

    private Brick GetBrickComponent(GameObject gameObject)
    {
        Brick brick = gameObject.GetComponent<Brick>();
        if (brick == null)
        {
            brick = gameObject.GetComponentInParent<Brick>();
        }
        return brick;
    }

    public void ResetBall()
    {
        isLaunched = false;
        transform.position = startPosition;
        rb.velocity = Vector2.zero;
        currentSpeed = initialSpeed;
    }

    public void LaunchBall(Vector2 direction)
    {
        isLaunched = true;
        rb.velocity = direction.normalized * currentSpeed;
        Debug.Log("Ball launched with velocity: " + rb.velocity);
    }
}