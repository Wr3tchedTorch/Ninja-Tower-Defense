using Godot;

public partial class tower : Area2D
{
    public Area2D CurrentEnemy;
    public bool CanAttack = true;
    private bool _canBuild = true;
    public bool IsBuilt = false;
    [Export]
    public string TargetProjectilePath;
    [Export]
    public double AttackSpeed;

    public override void _Ready()
    {
        GD.Print("Main class script ready");

        GetNode<Timer>("AttackDelay").WaitTime = 3/AttackSpeed;

        GetNode<Sprite2D>("%RangeDraw").GlobalPosition = GetGlobalMousePosition();
        GetNode<Sprite2D>("%RangeDraw").Visible = true;

        SetRangeSpriteRadius();
        Global.IsBuilding = true;

        Tween idleTween = CreateTween().SetLoops();
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 0.9f), 1);
        idleTween.TweenProperty(GetNode<Node2D>("Sprite"), "scale", new Vector2(1, 1.2f), 1);
    }

    public override void _Process(double delta)
    {
        GetNode<Sprite2D>("%RangeDraw").GlobalPosition = GlobalPosition;

        if (GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying()) return;

        if (!IsBuilt)
        {
            var collidingAreas = GetOverlappingAreas();
            _canBuild = collidingAreas.Count > 0 ? false : true;

            BuildingTower();
            return;
        }
        
        HandleMouseHovering();
    }

    private void BuildingTower()
    {
        GetNode<Sprite2D>("%RangeDraw").Visible = true;
        GlobalPosition = GetGlobalMousePosition();

        GetNode<Sprite2D>("%RangeDraw").Modulate = new Color(Colors.Blue, .3f);
        if (!_canBuild) GetNode<Sprite2D>("%RangeDraw").Modulate = new Color(Colors.Red, .3f);

        if (!Input.IsActionJustPressed("mb-left") || !_canBuild) return;
        IsBuilt = true;
        Global.IsBuilding = false;
        GetNode<Sprite2D>("%RangeDraw").Visible = false;
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
            GetNode<Sprite2D>("%RangeDraw").Visible = true;
            return;
        }        
        GetNode<Sprite2D>("%RangeDraw").Visible = false;
    }

    private void SetRangeSpriteRadius()
    {
        float currentRadius = ((CircleShape2D)GetNode<CollisionShape2D>("%AttackCircle").Shape).Radius;
        float newWidth = (currentRadius * 3.561f) / 140;
        float newHeight = (currentRadius * 3.52f) / 140;

        Vector2 newScale = new Vector2(newWidth, newHeight);
        GetNode<Sprite2D>("%RangeDraw").Scale = newScale;
    }

    public void OnAttackDelayTimeout() => CanAttack = true;
}
