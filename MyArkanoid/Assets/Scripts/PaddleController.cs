using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PaddleController : MonoBehaviour
{
    public float paddleSpeed = 10f;
    public float paddleWidth = 2f;
    public Slider controlSlider;
    public float maxBounceAngle = 75f;

    [Tooltip("Vertical offset from the bottom of the screen. Positive values move the paddle upwards.")]
    public float verticalOffset = 0.5f;

    [Tooltip("Horizontal offset from the screen edges. Lower values reduce the gap.")]
    public float horizontalEdgeOffset = 0.1f;

    private float minX;
    private float maxX;
    private Camera mainCamera;
    private Vector3 originalScale;
    private Coroutine expandCoroutine;
    private BallController ballController;
    private bool hasLaunchedBall = false;

    // Declare the collider variables
    private BoxCollider2D physicsCollider;
    private BoxCollider2D triggerCollider;

    private void Awake()
    {
        // Ensure the paddle has the correct tag
        gameObject.tag = "Paddle";

        // Set up the physics collider
        physicsCollider = GetComponent<BoxCollider2D>();
        if (physicsCollider == null)
        {
            physicsCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        physicsCollider.isTrigger = false;

        // Set up the trigger collider
        triggerCollider = gameObject.AddComponent<BoxCollider2D>();
        triggerCollider.isTrigger = true;
        // Make the trigger collider slightly larger than the physics collider
        triggerCollider.size = physicsCollider.size * 1.1f;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        originalScale = transform.localScale;

        ballController = FindObjectOfType<BallController>();
        if (ballController == null)
        {
            Debug.LogError("BallController not found in the scene!");
        }

        SetupSlider();
        UpdatePaddleBoundaries();
    }

    private void SetupSlider()
    {
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

    private void UpdatePaddleBoundaries()
    {
        if (mainCamera != null)
        {
            float screenAspect = (float)Screen.width / Screen.height;
            float cameraHeight = mainCamera.orthographicSize * 2;
            float cameraWidth = cameraHeight * screenAspect;
            float halfPaddleWidth = paddleWidth / 2f;

            // Adjust minX and maxX using horizontalEdgeOffset
            minX = -cameraWidth / 2 + halfPaddleWidth + horizontalEdgeOffset;
            maxX = cameraWidth / 2 - halfPaddleWidth - horizontalEdgeOffset;

            // Update paddle Y position using the verticalOffset
            Vector3 paddlePos = transform.position;
            paddlePos.y = -mainCamera.orthographicSize + verticalOffset;
            transform.position = paddlePos;

            // Update the slider's width to match the paddle's movement range
            //if (controlSlider != null)
            //{
            //    RectTransform sliderRect = controlSlider.GetComponent<RectTransform>();
            //    if (sliderRect != null)
            //    {
            //        float sliderWidth = maxX - minX;
            //        sliderRect.sizeDelta = new Vector2(sliderWidth, sliderRect.sizeDelta.y);
            //    }
            //}
        }
    }

    private void Update()
    {
        // Check for screen size changes
        if (Screen.width != mainCamera.pixelWidth || Screen.height != mainCamera.pixelHeight)
        {
            UpdatePaddleBoundaries();
        }
        UpdateVerticalPosition();
        UpdateHorizontalBoundaries();
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

        if (!hasLaunchedBall && ballController != null)
        {
            ballController.StartLaunchSequence();
            hasLaunchedBall = true;
        }
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
            Vector3 paddleCenter = transform.position;

            float offset = hitPoint.x - paddleCenter.x;
            float width = paddleWidth / 2f;
            float currentAngle = Vector2.SignedAngle(Vector2.up, ballRb.velocity);
            float bounceAngle = (offset / width) * maxBounceAngle;
            float newAngle = Mathf.Clamp(currentAngle + bounceAngle, -maxBounceAngle, maxBounceAngle);

            Quaternion rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
            ballRb.velocity = rotation * Vector2.up * ballRb.velocity.magnitude;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Paddle triggered by {other.name}");
        PowerUp powerUp = other.GetComponent<PowerUp>();
        if (powerUp != null)
        {
            powerUp.Activate(this);
            Destroy(other.gameObject);
        }
    }

    public void ExpandPaddle(float expandFactor, float duration)
    {
        if (expandCoroutine != null)
        {
            StopCoroutine(expandCoroutine);
        }
        expandCoroutine = StartCoroutine(ExpandPaddleCoroutine(expandFactor, duration));
    }

    private IEnumerator ExpandPaddleCoroutine(float expandFactor, float duration)
    {
        Vector3 expandedScale = originalScale;
        expandedScale.x *= expandFactor;
        transform.localScale = expandedScale;

        physicsCollider.size = new Vector2(physicsCollider.size.x * expandFactor, physicsCollider.size.y);
        triggerCollider.size = physicsCollider.size * 1.1f;

        paddleWidth *= expandFactor;
        UpdatePaddleBoundaries();

        yield return new WaitForSeconds(duration);

        transform.localScale = originalScale;
        paddleWidth /= expandFactor;

        physicsCollider.size = new Vector2(physicsCollider.size.x / expandFactor, physicsCollider.size.y);
        triggerCollider.size = physicsCollider.size * 1.1f;

        UpdatePaddleBoundaries();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, transform.position.y, 0), new Vector3(maxX, transform.position.y, 0));
    }

    public void OnBallReset()
    {
        hasLaunchedBall = false;
        controlSlider.value = 0.5f;
    }

    public void UpdateVerticalPosition()
    {
        UpdatePaddleBoundaries();
    }

    public void UpdateHorizontalBoundaries()
    {
        UpdatePaddleBoundaries();
    }
}