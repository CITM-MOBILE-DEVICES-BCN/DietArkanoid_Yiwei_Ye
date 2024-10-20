using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    public static BoundaryManager Instance { get; private set; }

    [SerializeField] private GameObject topBoundary;
    [SerializeField] private GameObject leftBoundary;
    [SerializeField] private GameObject rightBoundary;
    [SerializeField] private GameObject bottomBoundary;

    [Header("Boundary Offsets")]
    [SerializeField] private float topOffset = 0f;
    [SerializeField] private float leftOffset = 0f;
    [SerializeField] private float rightOffset = 0f;
    [SerializeField] private float bottomOffset = 0f;

    private Camera mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        mainCamera = Camera.main;
        SetupBoundaries();
    }

    private void Update()
    {
        UpdateBoundaries();
    }

    private void SetupBoundaries()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        float screenAspect = (float)Screen.width / Screen.height;
        float cameraHeight = mainCamera.orthographicSize * 2;
        float cameraWidth = cameraHeight * screenAspect;

        // Top boundary
        SetBoundaryTransform(topBoundary, new Vector2(cameraWidth, 1), new Vector3(0, mainCamera.orthographicSize + topOffset, 0));

        // Bottom boundary
        SetBoundaryTransform(bottomBoundary, new Vector2(cameraWidth, 1), new Vector3(0, -mainCamera.orthographicSize + bottomOffset, 0));

        // Left boundary
        SetBoundaryTransform(leftBoundary, new Vector2(1, cameraHeight), new Vector3(-cameraWidth / 2 + leftOffset, 0, 0));

        // Right boundary
        SetBoundaryTransform(rightBoundary, new Vector2(1, cameraHeight), new Vector3(cameraWidth / 2 + rightOffset, 0, 0));
    }

    private void SetBoundaryTransform(GameObject boundary, Vector2 scale, Vector3 position)
    {
        if (boundary != null)
        {
            boundary.transform.localScale = scale;
            boundary.transform.position = position;
        }
        else
        {
            Debug.LogWarning($"Boundary object not assigned: {boundary.name}");
        }
    }

    public bool IsBottomBoundary(Collision2D collision)
    {
        return collision.gameObject == bottomBoundary;
    }

    public void UpdateBoundaries()
    {
        SetupBoundaries();
    }
}