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

        // Ensure the slider is set up correctly
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
        // Calculate the new X position based on the slider value
        float newX = Mathf.Lerp(minX, maxX, value);

        // Update the paddle position
        Vector3 newPosition = transform.position;
        newPosition.x = newX;
        transform.position = newPosition;
    }

    // Optional: Add a method for automatic movement (as mentioned in the instructions)
    public void AutoMove(float targetX)
    {
        targetX = Mathf.Clamp(targetX, minX, maxX);
        float sliderValue = Mathf.InverseLerp(minX, maxX, targetX);
        controlSlider.value = sliderValue;
    }

    private void OnDrawGizmos()
    {
        // Visualize the paddle's movement range in the editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y, 0), new Vector3(maxX, transform.position.y, 0));
    }
}