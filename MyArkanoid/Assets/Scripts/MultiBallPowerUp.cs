using UnityEngine;

public class MultiBallPowerUp : PowerUp
{
    public int additionalBalls = 2;

    protected override void Activate()
    {
        BallController originalBall = FindObjectOfType<BallController>();
        if (originalBall != null)
        {
            for (int i = 0; i < additionalBalls; i++)
            {
                BallController newBall = Instantiate(originalBall, originalBall.transform.position, Quaternion.identity);
                newBall.LaunchBall(Random.insideUnitCircle.normalized);
            }
        }
    }
}