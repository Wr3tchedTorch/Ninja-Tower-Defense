using Godot;
using System;

public partial class camera : Camera2D
{

    [Export]
    public float ShakeFade = 5;

    RandomNumberGenerator rng = new RandomNumberGenerator();

    public float ShakeStrength = 0;

    public override void _Process(double delta)
    {
        if (ShakeStrength > 0)
        {
            ShakeStrength = Mathf.Lerp(ShakeStrength, 0, ShakeFade * (float)delta);
            Offset += RandomOffset();
        }
        if (ShakeStrength < 1)
        {
            Tween camOffsetTween = GetTree().CreateTween();
            camOffsetTween.TweenProperty(GetNode<Camera2D>("%Camera2D"), "offset", new Vector2(0, 0), 0.2);
        }
        GD.Print("cam offset: " + Offset);
    }

    public void ApplyShake(float RandomStrength = 6)
    {
        ShakeStrength = RandomStrength;
    }

    public Vector2 RandomOffset()
    {
        return new Vector2(rng.RandfRange(-ShakeStrength, ShakeStrength), rng.RandfRange(-ShakeStrength, ShakeStrength));
    }
}
