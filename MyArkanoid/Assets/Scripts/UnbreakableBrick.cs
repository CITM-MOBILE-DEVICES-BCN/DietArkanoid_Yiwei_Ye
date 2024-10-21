using UnityEngine;

public class UnbreakableBrick : Brick
{
    protected override void Awake()
    {
        base.Awake();
        isBreakable = false;
        hitPoints = int.MaxValue;
        scoreValue = 0;
    }

    public override void Hit()
    {
        // Unbreakable brick doesn't take damage, but we might want to play a sound or show a visual effect
    }

    public override bool ShouldBeDestroyed()
    {
        return false; // UnbreakableBrick should never be destroyed
    }
}
