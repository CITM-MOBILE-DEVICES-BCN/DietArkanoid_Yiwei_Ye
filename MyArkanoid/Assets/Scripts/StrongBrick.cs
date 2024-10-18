using UnityEngine;

public class StrongBrick : Brick
{
    protected override void Awake()
    {
        base.Awake();
        hitPoints = 2;
        scoreValue = 20;
    }
}
