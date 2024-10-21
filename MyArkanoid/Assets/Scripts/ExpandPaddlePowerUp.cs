using UnityEngine;

public class ExpandPaddlePowerUp : PowerUp
{
    public float expandFactor = 1.5f;
    private float duration = 5.0f;


    public float Duration
    {
        get { return duration; }
        set { duration = Mathf.Max(0.1f, value); }  // Ensure duration is always positive
    }

    public override void Activate(PaddleController paddle)
    {
        paddle.ExpandPaddle(expandFactor, duration);
        Debug.Log($"Expand Paddle Power-Up activated! Duration: {duration} seconds");
    }
}