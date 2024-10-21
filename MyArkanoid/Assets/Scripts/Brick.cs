using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] protected int baseHitPoints = 1;
    [SerializeField] protected int baseScoreValue = 10;
    [SerializeField] protected Color[] stateColors;
    [SerializeField] protected bool isBreakable = true;



    protected int hitPoints;
    protected int scoreValue;
    protected SpriteRenderer spriteRenderer;
    protected BrickManager brickManager;

    protected virtual void Awake()
    {
        Debug.Log($"Brick Awake: {gameObject.name}, Type: {GetType().Name}");
        spriteRenderer = GetComponent<SpriteRenderer>();
        brickManager = FindObjectOfType<BrickManager>();
        if (brickManager == null)
        {
            Debug.LogError("BrickManager not found in the scene!");
        }
        UpdateColor();
    }

    public virtual void SetDifficulty(int level)
    {
        hitPoints = baseHitPoints + (level - 1) / 3;
        scoreValue = baseScoreValue * level;
        UpdateColor();
    }

    public virtual void Hit()
    {
        if (isBreakable)
        {
            hitPoints--;
            if (hitPoints <= 0)
            {
                DestroyBrick();
            }
            else
            {
                UpdateColor();
            }
        }
    }
    public virtual void DestroyBrick()
    {
        if (brickManager != null)
        {
            brickManager.RemoveBrick(this);
        }
        else
        {
            Debug.LogError("BrickManager is null when trying to destroy brick");
        }
        GameManager.Instance.AddScore(scoreValue);
        Destroy(gameObject);
    }

    protected virtual void UpdateColor()
    {
        if (stateColors != null && stateColors.Length > 0)
        {
            int colorIndex = Mathf.Clamp(hitPoints - 1, 0, stateColors.Length - 1);
            spriteRenderer.color = stateColors[colorIndex];
        }
    }

    public virtual bool ShouldBeDestroyed()
    {
        return hitPoints <= 0;
    }

    public virtual bool IsBreakable()
    {
        return isBreakable && hitPoints > 0;
    }

}