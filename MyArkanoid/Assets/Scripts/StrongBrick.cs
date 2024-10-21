using UnityEngine;

public class StrongBrick : Brick
{
    protected override void Awake()
    {
        base.Awake();
        isBreakable = true;
        hitPoints = 2;
        scoreValue = 20;
    }
}
