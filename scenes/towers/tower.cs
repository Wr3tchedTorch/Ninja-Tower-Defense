using Godot;

public partial class tower : Area2D
{
    Area2D currentEnemy;
    private bool _canAttack = true;
    private bool _canBuild = true;
    private bool _isBuilt = false;

    public override void _Ready()
    {
        SetRangeSpriteRadius();
        Global.IsBuilding = true;

        Tween idleTween = CreateTween().SetLoops();
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 0.9f), 1);
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 1.2f), 1);
    }

    public override void _Process(double delta)
    {
        if (GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying()) return;

        if (!_isBuilt)
        {
            var collidingAreas = GetOverlappingAreas();
            _canBuild = collidingAreas.Count > 0 ? false : true;

            BuildingTower();
            return;
        }

        HandleTowerRangedAttack();
        HandleMouseHovering();
    }

    private void BuildingTower()
    {
        GetNode<Sprite2D>("RangeDraw").Visible = true;
        GlobalPosition = GetGlobalMousePosition();

        GetNode<Sprite2D>("RangeDraw").Modulate = new Color(Colors.Blue, .3f);
        if (!_canBuild) GetNode<Sprite2D>("RangeDraw").Modulate = new Color(Colors.Red, .3f);

        if (!Input.IsActionJustPressed("mb-left") || !_canBuild) return;
        _isBuilt = true;
        Global.IsBuilding = false;
        GetNode<Sprite2D>("RangeDraw").Visible = false;
        GetNode<AnimationPlayer>("AnimationPlayer").Play("spawn");
    }

    private void HandleMouseHovering()
    {
        if (Global.IsBuilding) return;

        float hitBoxW = ((RectangleShape2D)GetNode<CollisionShape2D>("HitBox").Shape).Size.X;
        float hitBoxH = ((RectangleShape2D)GetNode<CollisionShape2D>("HitBox").Shape).Size.Y;
        Vector2 hitBoxPos = GetNode<CollisionShape2D>("HitBox").GlobalPosition;

        if (GetGlobalMousePosition().X > hitBoxPos.X - hitBoxW / 2 && GetGlobalMousePosition().X < hitBoxPos.X + hitBoxW / 2 &&
            GetGlobalMousePosition().Y > hitBoxPos.Y - hitBoxH / 2 && GetGlobalMousePosition().Y < hitBoxPos.Y + hitBoxH / 2
            )
        {
            Input.SetDefaultCursorShape(Input.CursorShape.PointingHand);
            GetNode<Sprite2D>("RangeDraw").Visible = true;
            return;
        }        
        GetNode<Sprite2D>("RangeDraw").Visible = false;
    }

    private void HandleTowerRangedAttack()
    {
        Area2D range = GetNode<Area2D>("AttackRange");

        if (!_canAttack) return;        
        var enemies = range.GetOverlappingAreas();

        if (enemies.Count <= 0 || !IsInstanceValid(enemies[0])) return;
        currentEnemy = enemies[0];

        PackedScene bulletScene = GD.Load<PackedScene>("res://scenes/weapons/projectile.tscn");
        projectile newProjectile = bulletScene.Instantiate<projectile>();

        GetNode<Node2D>("../Projectiles").AddChild(newProjectile);
        newProjectile.GlobalPosition = GetNode<Marker2D>("Marker2D").GlobalPosition;
        GD.Print(IsInstanceValid(currentEnemy));
        newProjectile.Target = currentEnemy;

        Tween rotTween = CreateTween();
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees - 45), .2f);
        rotTween.TweenProperty(GetNode<Node2D>("Sprite"), "rotation", Mathf.DegToRad(RotationDegrees), .2f);

        _canAttack = false;
        GetNode<Timer>("AttackDelay").Start();
    }

    private void SetRangeSpriteRadius()
    {
        float currentRadius = ((CircleShape2D)GetNode<CollisionShape2D>("%AttackCircle").Shape).Radius;
        float newWidth = (currentRadius * 3.561f) / 140;
        float newHeight = (currentRadius * 3.52f) / 140;

        Vector2 newScale = new Vector2(newWidth, newHeight);
        GetNode<Sprite2D>("RangeDraw").Scale = newScale;
    }

    public void OnAttackDelayTimeout() => _canAttack = true;
}
