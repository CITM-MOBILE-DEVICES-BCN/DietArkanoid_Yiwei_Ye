using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    public float fallSpeed = 2f;

    protected virtual void Update()
    {
        // Make the power-up fall
        transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

        // Destroy if it goes off-screen
        if (transform.position.y < -Camera.main.orthographicSize)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Paddle"))
        {
            Activate();
            Destroy(gameObject);
        }
    }

    protected abstract void Activate();
}