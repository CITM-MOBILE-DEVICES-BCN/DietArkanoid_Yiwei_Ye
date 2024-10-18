using UnityEngine;

public class Brick : MonoBehaviour
{
    [SerializeField] protected int hitPoints = 1;
    [SerializeField] protected int scoreValue = 10;
    [SerializeField] protected Color[] stateColors;

    protected SpriteRenderer spriteRenderer;
    protected BrickManager brickManager;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        brickManager = FindObjectOfType<BrickManager>();
        UpdateColor();
    }

    public virtual void Hit()
    {
        hitPoints--;
        if (hitPoints <= 0)
        {
            Destroy();
        }
        else
        {
            UpdateColor();
        }
        Debug.Log("Hit points: " + hitPoints);
    }

    protected virtual void Destroy()
    {
        GameManager.Instance.AddScore(scoreValue);
        if (brickManager != null)
        {
            brickManager.RemoveBrick(this);
        }
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
}