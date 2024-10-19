using UnityEngine;

public class MultiBallPowerUp : PowerUp
{
    public int additionalBalls = 2;

    public override void Activate(PaddleController paddle)
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
        Debug.Log("Multi-Ball Power-Up activated!");
    }
}