using UnityEngine;
using UnityEngine.UI;

public class PaddleController : MonoBehaviour
{
    public float paddleSpeed = 10f;
    public float paddleWidth = 2f;
    public Slider controlSlider;

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

    private void Update()
    {
        // Allow keyboard input for paddle movement
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0)
        {
            MovePaddle(horizontalInput);
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
        MovePaddleToPosition(Mathf.Lerp(minX, maxX, value));
    }

    private void MovePaddle(float direction)
    {
        Vector3 newPosition = transform.position + Vector3.right * direction * paddleSpeed * Time.deltaTime;
        MovePaddleToPosition(newPosition.x);
    }

    private void MovePaddleToPosition(float xPosition)
    {
        xPosition = Mathf.Clamp(xPosition, minX, maxX);
        Vector3 newPosition = transform.position;
        newPosition.x = xPosition;
        transform.position = newPosition;

        // Update slider value
        if (controlSlider != null)
        {
            controlSlider.value = Mathf.InverseLerp(minX, maxX, xPosition);
        }
    }

    public void AutoMove(float targetX)
    {
        MovePaddleToPosition(targetX);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y, 0), new Vector3(maxX, transform.position.y, 0));
    }
}