using UnityEngine;
using UnityEngine.UI;

public class PaddleController : MonoBehaviour
{
    public float paddleSpeed = 10f;
    public float paddleWidth = 2f;
    public Slider controlSlider;
    public float maxBounceAngle = 75f;

    private float minX;
    private float maxX;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateBoundaries();

        if (controlSlider != null)
        {
            controlSlider.minValue = 0f;
            controlSlider.maxValue = 1f;
            controlSlider.value = 0.5f;
            controlSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        else
        {
            Debug.LogError("Control slider not assigned to PaddleController!");
        }
    }

    private void CalculateBoundaries()
    {
        if (mainCamera != null)
        {
            float halfPaddleWidth = paddleWidth / 2f;
            float screenAspect = (float)Screen.width / Screen.height;
            float cameraHeight = mainCamera.orthographicSize * 2;
            float cameraWidth = cameraHeight * screenAspect;

            minX = -cameraWidth / 2 + halfPaddleWidth;
            maxX = cameraWidth / 2 - halfPaddleWidth;
        }
    }

    private void OnSliderValueChanged(float value)
    {
        float newX = Mathf.Lerp(minX, maxX, value);
        Vector3 newPosition = transform.position;
        newPosition.x = newX;
        transform.position = newPosition;
    }

    public void AutoMove(float targetX)
    {
        targetX = Mathf.Clamp(targetX, minX, maxX);
        float sliderValue = Mathf.InverseLerp(minX, maxX, targetX);
        controlSlider.value = sliderValue;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            Rigidbody2D ballRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 paddleCenter = new Vector3(transform.position.x, transform.position.y);

            float offset = hitPoint.x - paddleCenter.x;
            float width = paddleWidth / 2f;
            float currentAngle = Vector2.SignedAngle(Vector2.up, ballRb.velocity);
            float bounceAngle = (offset / width) * maxBounceAngle;
            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -maxBounceAngle, maxBounceAngle);

            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            ballRb.velocity = rotation * Vector2.up * ballRb.velocity.magnitude;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y, 0), new Vector3(maxX, transform.position.y, 0));
    }
}