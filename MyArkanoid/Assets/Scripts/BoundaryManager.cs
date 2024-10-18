using UnityEngine;

public class BoundaryManager : MonoBehaviour
{
    public static BoundaryManager Instance { get; private set; }

    [SerializeField] private GameObject topBoundary;
    [SerializeField] private GameObject leftBoundary;
    [SerializeField] private GameObject rightBoundary;
    [SerializeField] private GameObject bottomBoundary;

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
        SetBoundaryTransform(topBoundary, new Vector2(cameraWidth, 1), new Vector3(0, mainCamera.orthographicSize, 0));

        // Left boundary
        SetBoundaryTransform(leftBoundary, new Vector2(1, cameraHeight), new Vector3(-cameraWidth / 2, 0, 0));

        // Right boundary
        SetBoundaryTransform(rightBoundary, new Vector2(1, cameraHeight), new Vector3(cameraWidth / 2, 0, 0));

        // Bottom boundary
        SetBoundaryTransform(bottomBoundary, new Vector2(cameraWidth, 1), new Vector3(0, -mainCamera.orthographicSize, 0));
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
        bool isBottom = collision.gameObject == bottomBoundary;
        if (isBottom)
        {
            Debug.Log("Ball hit bottom boundary");
        }
        return isBottom;
    }

    public bool IsBelowBottomBoundary(Vector3 position)
    {
        return position.y < bottomBoundary.transform.position.y;
    }
}