using UnityEngine;

public class MultiBallPowerUp : PowerUp
{
    public int additionalBalls = 2;

    public override void Activate(PaddleController paddle)
    {
        BallController originalBall = FindObjectOfType<BallController>();
        if (originalBall != null)
        {
            for (int i = 0; i < additionalBalls; i++)
            {
                BallController newBall = Instantiate(originalBall, originalBall.transform.position, Quaternion.identity);

                // Ensure the new ball has a Rigidbody2D component
                Rigidbody2D rb = newBall.GetComponent<Rigidbody2D>();
                if (rb == null)
                {
                    rb = newBall.gameObject.AddComponent<Rigidbody2D>();
                }
                rb.velocity = Random.insideUnitCircle.normalized * originalBall.GetComponent<Rigidbody2D>().velocity.magnitude;

                // Add a component to destroy the ball when it touches the bottom boundary
                newBall.gameObject.AddComponent<DestroyOnBottomBoundary>();
            }
        }
        Debug.Log($"Multi-Ball Power-Up activated! Spawned {additionalBalls} additional balls.");
    }
}

public class DestroyOnBottomBoundary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomBoundary"))
        {
            Debug.Log($"Ball {gameObject.name} touched bottom boundary. Destroying.");
            Destroy(gameObject);
        }
    }
}