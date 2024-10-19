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
    public int rows = 5;
    public int columns = 10;
    public float horizontalSpacing = 0.2f;
    public float verticalSpacing = 0.2f;
    public float powerUpChance = 0.1f;

    private List<Brick> activeBricks = new List<Brick>();

    private void Start()
    {
        CreateBrickField();
    }

    private void CreateBrickField()
    {
        Vector2 startPosition = CalculateStartPosition();

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector2 position = new Vector2(
                    startPosition.x + col * (1 + horizontalSpacing),
                    startPosition.y - row * (1 + verticalSpacing)
                );

                CreateBrick(position);
            }
        }
    }

    private Vector2 CalculateStartPosition()
    {
        float totalWidth = columns * (1 + horizontalSpacing) - horizontalSpacing;
        float totalHeight = rows * (1 + verticalSpacing) - verticalSpacing;

        return new Vector2(
            -totalWidth / 2f,
            Camera.main.orthographicSize - 1 - verticalSpacing
        );
    }

    private void CreateBrick(Vector2 position)
    {
        GameObject brickPrefab = ChooseBrickType();
        GameObject brickObject = Instantiate(brickPrefab, position, Quaternion.identity, transform);
        Brick brick = brickObject.GetComponent<Brick>();
        if (brick != null)
        {
            activeBricks.Add(brick);
            Debug.Log($"Brick created: {brick.name}, Type: {brick.GetType().Name}, Position: {position}");
        }
        else
        {
            Debug.LogError($"Brick component not found on instantiated object: {brickObject.name}");
        }
    }

    private GameObject ChooseBrickType()
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

        // Default to the first brick type if probabilities don't sum to 1
        return brickTypes[0].prefab;
    }

    public void RemoveBrick(Brick brick)
    {
        activeBricks.Remove(brick);
        Debug.Log($"Brick removed: {brick.name}. Remaining bricks: {activeBricks.Count}");
        if (activeBricks.Count == 0)
        {
            Debug.Log("All bricks destroyed. Advancing level.");
            GameManager.Instance.AdvanceLevel();
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
                Instantiate(powerUpPrefab, position, Quaternion.identity);
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
}