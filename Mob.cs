using Godot;
using System;

public partial class Mob : PathFollow2D
{
    [Export]
    public float Speed = 50;
    [Export]
    public int Damage = 10;

    public int Health = 100;

    public override void _Ready()
    {
        Random rng = new Random();
        Speed = (float)rng.NextDouble() * Speed / 2 + Speed;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("walk_front");

    }

    public override void _PhysicsProcess(double delta)
    {
        if (GetNode<Sprite2D>("Sprite2D").Modulate == new Color(Colors.White, 0))
        {
            QueueFree();
        }

        Progress += Speed * (float)delta;
    }

    public void OnArea2dAreaEntered(Area2D area)
    {
        if (area is projectile)
        {
            return;
        }
        else
        {
            Tween fadeEnemyOut = CreateTween();
            fadeEnemyOut.TweenProperty(GetNode<Sprite2D>("Sprite2D"), "modulate", new Color(Colors.White, 0), .2);
        }
    }

    public void Hit(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("death");
        }

        Tween healthTween = CreateTween();
        healthTween.TweenProperty(GetNode<ProgressBar>("HealthBar"), "value", Health, .2f);
    }
}
