using UnityEngine;

public class ExpandPaddlePowerUp : PowerUp
{
    public float expandFactor = 1.5f;
    public float duration = 10f;

    public override void Activate(PaddleController paddle)
    {
        paddle.ExpandPaddle(expandFactor, duration);
        Debug.Log("Expand Paddle Power-Up activated!");
    }
}