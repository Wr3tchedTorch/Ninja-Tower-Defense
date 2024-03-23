using Godot;
using System;

public partial class ranged_tower : tower
{
    public override void _Process(double delta) {
        base._Process(delta);
        if (IsBuilt) HandleTowerRangedAttack();
    }

    public void HandleTowerRangedAttack()
    {
        Area2D range = GetNode<Area2D>("AttackRange");

        if (!CanAttack) return;
        var enemies = range.GetOverlappingAreas();

        if (enemies.Count <= 0 || !IsInstanceValid(enemies[0])) return;
        CurrentEnemy = enemies[0];

        PackedScene bulletScene = GD.Load<PackedScene>(TargetProjectilePath);
        projectile newProjectile = bulletScene.Instantiate<projectile>();

        GetNode<Node2D>("/root/Level/Projectiles").AddChild(newProjectile);
        newProjectile.GlobalPosition = GetNode<Marker2D>("Marker2D").GlobalPosition;
        newProjectile.Target = CurrentEnemy;

        Tween rotTween = CreateTween();
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees - 45), .2f);
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees), .2f);

        CanAttack = false;
        GetNode<Timer>("AttackDelay").Start();
    }
}
