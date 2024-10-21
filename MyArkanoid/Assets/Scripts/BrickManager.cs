using UnityEngine;
using System.Collections.Generic;

public class BrickManager : MonoBehaviour
{
    [System.Serializable]
    public class BrickType
    {
        public GameObject prefab;
        public float probability;
    }

    [System.Serializable]
    public class PowerUpType
    {
        public GameObject prefab;
        public float probability;
    }

    public List<BrickType> brickTypes;
    public List<PowerUpType> powerUpTypes;
    public int baseRows = 5;
    public int baseColumns = 10;
    public float horizontalSpacing = 0.2f;
    public float verticalSpacing = 0.2f;
    public float powerUpChance = 0.1f;

    private List<Brick> activeBricks = new List<Brick>();


    private void Start()
    {
        //CreateBrickField();
    }
    public void ResetBricks(int level)
    {
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
    }

    private void CreateBrickField(int level)
    {
        int rows = baseRows + (level - 1) / 2;
        int columns = baseColumns + (level - 1) / 2;

        Vector2 startPosition = CalculateStartPosition(rows, columns);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + col * (1 + horizontalSpacing),
                    startPosition.y - row * (1 + verticalSpacing)
                );

                CreateBrick(position, level);
            }
        }

        Debug.Log($"Created brick field with {rows} rows and {columns} columns for level {level}");
    }

    private Vector2 CalculateStartPosition(int rows, int columns)
    {
        float totalWidth = columns * (1 + horizontalSpacing) - horizontalSpacing;
        float totalHeight = rows * (1 + verticalSpacing) - verticalSpacing;

        return new Vector2(
            -totalWidth / 2f,
            Camera.main.orthographicSize - 1 - verticalSpacing
        );
    }



    private void CreateBrick(Vector2 position, int level)
    {
        GameObject brickPrefab = ChooseBrickType(level);
        GameObject brickObject = Instantiate(brickPrefab, position, Quaternion.identity, transform);

        Brick brick = brickObject.GetComponent<Brick>();
        if (brick == null)
        {
            Debug.LogError($"Brick component not found on instantiated object: {brickObject.name}");
            return;
        }

        brick.SetDifficulty(level);
        activeBricks.Add(brick);
    }

    private GameObject ChooseBrickType(int level)
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
        if (activeBricks.Count == 0)
        {
            GameManager.Instance.LevelCompleted();
        }
        else
        {
            TrySpawnPowerUp(brick.transform.position);
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

                ExpandPaddlePowerUp powerUp = instance.GetComponent<ExpandPaddlePowerUp>();

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
}