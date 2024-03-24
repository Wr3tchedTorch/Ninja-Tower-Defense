using Godot;

public partial class fire_mage_tower : tower
{
    private bool _createdProjectile = false;
    Vector2 _scaleIncreaseValue = new Vector2(0.1f, 0.1f);
    private projectile newProjectile;

    public override void _Process(double delta)
    {
        if (IsBuilt) HandleTowerRangedAttack();
        base._Process(delta);
    }

    public void HandleTowerRangedAttack()
    {
        Area2D range = GetNode<Area2D>("AttackRange");

        if (!_createdProjectile)
        {
            PackedScene bulletScene = GD.Load<PackedScene>(TargetProjectilePath);
            newProjectile = bulletScene.Instantiate<projectile>();
            newProjectile.HasBeenThrowed = false;

            GetNode<Node2D>("/root/Level/Projectiles").AddChild(newProjectile);
            newProjectile.GlobalPosition = GetNode<Marker2D>("Marker2D").GlobalPosition;
            _createdProjectile = true;
        }
                
        double newValue = 0.1;
        newValue = Utils.Clamp(20/GetNode<Timer>("AttackDelay").TimeLeft, 0, 100);
        newValue = Mathf.Remap(newValue, 0, 100, 0, 2);
        newProjectile.Scale = new Vector2((float)newValue, (float)newValue);

        var enemies = range.GetOverlappingAreas();

        if (enemies.Count <= 0 || !IsInstanceValid(enemies[0]) || !CanAttack) return;
        CurrentEnemy = enemies[0];

        newProjectile.Target = CurrentEnemy;
        newProjectile.HasBeenThrowed = true;
        _createdProjectile = false;        

        Tween rotTween = CreateTween();
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees - 45), .2f);
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees), .2f);

        CanAttack = false;
        GetNode<Timer>("AttackDelay").Start();
    }
}
