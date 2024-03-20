using Godot;
using System;

public partial class tower : Area2D
{
    [Export]
    public int Damage = 50;
    Area2D currentEnemy;
    private bool _canAttack = true;

    public override void _Ready()
    {
        Tween idleTween = CreateTween().SetLoops();
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 0.9f), 1);
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 1.2f), 1);
    }

    public override void _Process(double delta)
    {        
        Area2D range = GetNode<Area2D>("AttackRange");
        var enemies = range.GetOverlappingAreas();

        if (enemies.Count > 0 && _canAttack) {
            currentEnemy = enemies[0];

            PackedScene bulletScene = GD.Load<PackedScene>("res://scenes/weapons/projectile.tscn");
            projectile newProjectile = bulletScene.Instantiate<projectile>();

            GetNode<Node2D>("../Projectiles").AddChild(newProjectile);
            newProjectile.GlobalPosition = GetNode<Marker2D>("Marker2D").GlobalPosition;
            newProjectile.Target = currentEnemy;

            Tween rotTween = CreateTween();
            rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees-45), .2f);
            rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees), .2f);

            GetNode<Timer>("AttackDelay").Start();
            _canAttack = false;
        }
    }

    public void OnAttackDelayTimeout() {
        _canAttack = true;
    }
}
