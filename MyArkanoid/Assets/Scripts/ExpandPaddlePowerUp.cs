using UnityEngine;

public class ExpandPaddlePowerUp : PowerUp
{
    public float expandFactor = 1.5f;
    public float duration = 10f;

    protected override void Activate()
    {
        PaddleController paddle = FindObjectOfType<PaddleController>();
        if (paddle != null)
        {
            paddle.ExpandPaddle(expandFactor, duration);
        }
    }
}