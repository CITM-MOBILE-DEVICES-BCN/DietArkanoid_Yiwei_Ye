using UnityEngine;

public class NormalBrick : Brick
{
    protected override void Awake()
    {
        base.Awake();
        isBreakable = true;
        hitPoints = 1;
        scoreValue = 10;
    }
}
