using Godot;
using System;

public partial class Mob : PathFollow2D
{
    [Export]
    public float Speed = 60;
    [Export]
    public int Damage = 3;
    [Export]
    public float resistence = 1;
    [Export]
    public static int SpawnChance = 100;
    public float Health = 100;

    public override void _Ready()
    {
        Random rng = new Random();
        Speed = (float)rng.NextDouble() * Speed / 2 + Speed;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("walk_front");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (GetNode<Sprite2D>("Sprite2D").Modulate == new Color(Colors.White, 0)) QueueFree();

        if (Health <= 0) return;
        Progress += Speed * (float)delta;
    }

    public void Hit(int damage)
    {
        Health -= damage / resistence;

        Tween healthTween = CreateTween();
        healthTween.TweenProperty(GetNode<ProgressBar>("HealthBar"), "value", Health, .1f);
        Callable callable = new Callable(this, "HealthBarTweenFinish");
        healthTween.Connect("finished", callable);
    }

    public void HealthBarTweenFinish()
    {
        if (Health > 0) return;

        GetNode<Area2D>("Area2D").Monitorable = false;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("death");
    }

    public void OnArea2dAreaEntered(Area2D area)
    {
        if (area is projectile) return;

        Tween fadeEnemyOut = CreateTween();
        fadeEnemyOut.TweenProperty(GetNode<Sprite2D>("Sprite2D"), "modulate", new Color(Colors.White, 0), .2);
    }
}
