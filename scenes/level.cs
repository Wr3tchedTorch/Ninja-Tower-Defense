using Godot;
using System;

public partial class level : Node2D
{
    private int _health = 100;
    private bool _gameoverScreenFaded = false;

    public override void _Process(double delta)
    {
        if (GetNode<ProgressBar>("%HealthBar").Value <= 0 && !_gameoverScreenFaded)
        {
            Gameover();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionPressed("reset-game"))
        {
            GetTree().ReloadCurrentScene();
        }
    }

    public void OnFinishLineAreaEntered(Area2D area)
    {
        if (area.GetParent() is Mob mob)
        {
            _health -= mob.Damage;
            Tween tweenHealth = CreateTween();
            tweenHealth.TweenProperty(GetNode<ProgressBar>("%HealthBar"), "value", _health, .2f);
        }
    }

    public void OnSpawnEnemyTimeout()
    {
        PackedScene mobScene = GD.Load<PackedScene>("res://scenes/enemies/mob.tscn");
        var newEnemy = mobScene.Instantiate();
        GetNode<Path2D>("Path2D").AddChild(newEnemy);
    }

    private void Gameover()
    {
        Tween tweenGameoverScreen = GetTree().CreateTween();

        tweenGameoverScreen.TweenProperty(GetNode<ColorRect>("%RedBackground"), "modulate", new Color(Colors.White, 1f), 1);
        tweenGameoverScreen.TweenProperty(GetNode<Label>("%GameoverLabel1"), "modulate", new Color(Colors.White, 1), 1);
        tweenGameoverScreen.TweenProperty(GetNode<Label>("%GameoverLabel2"), "modulate", new Color(Colors.Red, 1), 1);

        Callable callable = new Callable(this, "OnTweenFinished");
        tweenGameoverScreen.Connect("finished", callable);

        _gameoverScreenFaded = true;
    }

    private void OnTweenFinished() {
        GD.Print("tween finished");

        Tween blinkText = CreateTween().SetLoops();
        blinkText.TweenProperty(GetNode<Label>("%GameoverLabel3"), "modulate", new Color(Colors.White, 0), .7f);
        blinkText.TweenProperty(GetNode<Label>("%GameoverLabel3"), "modulate", new Color(Colors.White, 1), .7f);
    }
}
