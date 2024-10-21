using UnityEngine;
using System.Collections.Generic;

public class BrickManager : MonoBehaviour
{
    [System.Serializable]
    public class BrickType
    {
        public GameObject prefab;
        public float probability;
        public bool isBreakable;
    }

    [System.Serializable]
    public class PowerUpType
    {
        public GameObject prefab;
        public float probability;
    }

    [System.Serializable]
    public class LevelSettings
    {
        public List<BrickType> brickTypes;
        public float screenFillPercentage = 0.7f; // Percentage of screen height to fill with bricks
    }

    public List<LevelSettings> levelSettings;
    public List<PowerUpType> powerUpTypes;
    [SerializeField] private float horizontalSpacing = 0.1f;
    [SerializeField] private float verticalSpacing = 0.1f;
    public float powerUpChance = 0.1f;
    public float topOffset = 1f;
    public float sideOffset = 0.5f;

    private List<Brick> activeBricks = new List<Brick>();
    private int breakableBrickCount;
    private Vector2 brickSize;

    public Vector2 brickFieldPosition = new Vector2(0, 2); // New public field for brick field position
    private int currentLevel; // Track the current level


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBricks(currentLevel);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            DestroyAllBreakableBricks();
        }
        
    }
    public void DestroyAllBreakableBricks()
    {
        List<Brick> bricksToDestroy = new List<Brick>(activeBricks);
        foreach (Brick brick in bricksToDestroy)
        {
            if (brick != null && brick.IsBreakable())
            {
                brick.DestroyBrick();
            }
        }
    }

    public void ResetBricks(int level)
    {
        currentLevel = level;
        ClearBricks();
        CreateBrickField(level);
    }

    private void ClearBricks()
    {
        foreach (Brick brick in activeBricks)
        {
            if (brick != null)
            {
                Destroy(brick.gameObject);
            }
        }
        activeBricks.Clear();
        breakableBrickCount = 0;
    }

    private void CreateBrickField(int level)
    {
        LevelSettings currentLevelSettings = levelSettings[Mathf.Min(level - 1, levelSettings.Count - 1)];
        brickSize = GetBrickSize(currentLevelSettings.brickTypes[0].prefab);
        Vector2 playArea = CalculatePlayArea();

        int columns = CalculateColumns(playArea.x);
        int rows = CalculateRows(playArea.y, currentLevelSettings.screenFillPercentage);

        Vector2 startPosition = CalculateStartPosition(rows, columns, brickSize, playArea);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + col * (brickSize.x + horizontalSpacing),
                    startPosition.y - row * (brickSize.y + verticalSpacing)
                );

                position += brickFieldPosition; // Apply the brick field position offset

                CreateBrick(position, currentLevelSettings);
            }
        }

        Debug.Log($"Created brick field with {rows} rows and {columns} columns for level {level}. Breakable bricks: {breakableBrickCount}");
    }

    private Vector2 GetBrickSize(GameObject brickPrefab)
    {
        SpriteRenderer spriteRenderer = brickPrefab.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            return spriteRenderer.bounds.size;
        }
        return Vector2.one; // Default size if sprite renderer is not found
    }

    private Vector2 CalculatePlayArea()
    {
        float height = Camera.main.orthographicSize * 2;
        float width = height * Camera.main.aspect;
        return new Vector2(width - sideOffset * 2, height - topOffset);
    }

    private int CalculateColumns(float availableWidth)
    {
        float brickWidthWithSpacing = brickSize.x + horizontalSpacing;
        int columns = Mathf.FloorToInt(availableWidth / brickWidthWithSpacing);
        return Mathf.Max(1, columns); // Ensure at least one column
    }

    private int CalculateRows(float availableHeight, float screenFillPercentage)
    {
        float brickHeightWithSpacing = brickSize.y + verticalSpacing;
        float desiredHeight = availableHeight * screenFillPercentage;
        int rows = Mathf.FloorToInt(desiredHeight / brickHeightWithSpacing);
        return Mathf.Max(1, rows); // Ensure at least one row
    }

    private Vector2 CalculateStartPosition(int rows, int columns, Vector2 brickSize, Vector2 playArea)
    {
        float totalWidth = columns * brickSize.x + (columns - 1) * horizontalSpacing;
        float totalHeight = rows * brickSize.y + (rows - 1) * verticalSpacing;

        return new Vector2(
            -totalWidth / 2f,
            Camera.main.orthographicSize - topOffset - brickSize.y / 2
        );
    }

    private void CreateBrick(Vector2 position, LevelSettings levelSettings)
    {
        GameObject brickPrefab = ChooseBrickType(levelSettings.brickTypes);
        GameObject brickObject = Instantiate(brickPrefab, position, Quaternion.identity, transform);

        Brick brick = brickObject.GetComponent<Brick>();
        if (brick == null)
        {
            Debug.LogError($"Brick component not found on instantiated object: {brickObject.name}");
            return;
        }

        activeBricks.Add(brick);

        BrickType brickType = levelSettings.brickTypes.Find(bt => bt.prefab == brickPrefab);
        if (brickType != null && brickType.isBreakable)
        {
            breakableBrickCount++;
        }
    }

    private GameObject ChooseBrickType(List<BrickType> brickTypes)
    {
        float random = Random.value;
        float cumulativeProbability = 0f;

        foreach (BrickType brickType in brickTypes)
        {
            cumulativeProbability += brickType.probability;
            if (random <= cumulativeProbability)
            {
                return brickType.prefab;
            }
        }

        return brickTypes[0].prefab;
    }

    public void RemoveBrick(Brick brick)
    {
        activeBricks.Remove(brick);

        if (currentLevel <= levelSettings.Count)
        {
            BrickType brickType = levelSettings[currentLevel - 1].brickTypes
                .Find(bt => bt.prefab.name == brick.gameObject.name.Replace("(Clone)", "").Trim());

            if (brickType != null && brickType.isBreakable)
            {
                breakableBrickCount--;
                if (breakableBrickCount == 0)
                {
                    GameManager.Instance.LevelCompleted();
                }
            }

            TrySpawnPowerUp(brick.transform.position);
        }
        else
        {
            Debug.LogWarning($"Current level {currentLevel} exceeds the number of level settings.");
        }
    }

    public void UpdateBrickFieldPosition(Vector2 newPosition)
    {
        brickFieldPosition = newPosition;
        if (activeBricks.Count > 0)
        {
            ResetBricks(currentLevel);
        }
    }

    private void TrySpawnPowerUp(Vector2 position)
    {
        if (Random.value < powerUpChance)
        {
            GameObject powerUpPrefab = ChoosePowerUpType();
            if (powerUpPrefab != null)
            {
                var instance = Instantiate(powerUpPrefab, position, Quaternion.identity);
                Debug.Log($"Power-up spawned at {position}");
            }
        }
    }

    private GameObject ChoosePowerUpType()
    {
        float random = Random.value;
        float cumulativeProbability = 0f;

        foreach (PowerUpType powerUpType in powerUpTypes)
        {
            cumulativeProbability += powerUpType.probability;
            if (random <= cumulativeProbability)
            {
                return powerUpType.prefab;
            }
        }

        return null;
    }

    public void UpdateSpacing(float newHorizontalSpacing, float newVerticalSpacing)
    {
        horizontalSpacing = newHorizontalSpacing;
        verticalSpacing = newVerticalSpacing;

        if (activeBricks.Count > 0)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;
            ResetBricks(currentLevel);
        }
    }

    private void OnValidate()
    {
        // This ensures that the spacing is updated in the editor when changed
        if (Application.isPlaying && activeBricks.Count > 0)
        {
            UpdateSpacing(horizontalSpacing, verticalSpacing);
        }
    }
}