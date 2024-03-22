using System;
using Godot;

public partial class level : Node2D
{
    private int _health = 100;
    private bool _gameoverScreenFaded = false;
    private bool _menuIsOpen = false;
    private bool _closedMenuAfterBuying = false;
    private int enemiesLeft = 0;
    private int[] _waves = { 10, 15, 20, 25, 50 };
    private bool _waveStarted = false;
    private int _currentWaveIndex = 0;

    public override void _Ready()
    {
        Global.Money = 10;

        GetNode<ColorRect>("%RedBackground").Visible = false;

        GetNode<Timer>("%NextWaveDelay").Start();
        GetNode<Label>("%WaveLabel").Text = "Wave: " + (_currentWaveIndex + 1);
        _waveStarted = false;
    }
    public override void _Process(double delta)
    {
        if (Global.IsBuilding && !_closedMenuAfterBuying)
        {
            _closedMenuAfterBuying = true;
            OnButtonMenuPressed();
        }

        if (!Global.IsBuilding) _closedMenuAfterBuying = false;

        GetNode<Label>("%MoneyLabel").Text = "Money: " + Global.Money;

        if (Input.IsActionPressed("reset-game")) GetTree().ReloadCurrentScene();

        if (GetNode<ProgressBar>("%HealthBar").Value > 0 || _gameoverScreenFaded) return;
        Gameover();
    }

    private void Gameover()
    {
        Tween tweenGameoverScreen = GetTree().CreateTween();

        GetNode<ColorRect>("%RedBackground").Visible = true;
        tweenGameoverScreen.TweenProperty(GetNode<ColorRect>("%RedBackground"), "modulate", new Color(Colors.White, 1f), 1);
        tweenGameoverScreen.TweenProperty(GetNode<Label>("%GameoverLabel1"), "modulate", new Color(Colors.White, 1), 1);
        tweenGameoverScreen.TweenProperty(GetNode<Label>("%GameoverLabel2"), "modulate", new Color(Colors.Red, 1), 1);

        Callable callable = new Callable(this, "OnTweenFinished");
        tweenGameoverScreen.Connect("finished", callable);

        _gameoverScreenFaded = true;
    }

    private void StartNextWave()
    {
        Tween waveText = CreateTween();

        GetNode<Label>("%WaveWarning").Text = "Wave: " + (_currentWaveIndex + 1);
        GetNode<Label>("%WaveWarning").Modulate = new Color(Colors.White, 0);
        GetNode<Label>("%WaveWarning").Visible = true;

        waveText.TweenProperty(GetNode<Label>("%WaveWarning"), "modulate", new Color(Colors.White, 1), 1.5f);
        waveText.TweenInterval(1);
        waveText.TweenProperty(GetNode<Label>("%WaveWarning"), "modulate", new Color(Colors.White, 0), 1);
        
        GetNode<Label>("%WaveLabel").Text = "Wave: " + (_currentWaveIndex + 1);

        Callable callable = new Callable(this, "OnWaveTextTweenFinished");
        waveText.Connect("finished", callable);
    }

    private void OnWaveTextTweenFinished()
    {
        GetNode<Label>("%WaveWarning").Visible = false;
        _waveStarted = true;
    }

    private void OnTweenFinished()
    {
        Tween blinkText = CreateTween().SetLoops();

        blinkText.TweenProperty(GetNode<Label>("%GameoverLabel3"), "modulate", new Color(Colors.White, 0), .7f);
        blinkText.TweenProperty(GetNode<Label>("%GameoverLabel3"), "modulate", new Color(Colors.White, 1), .7f);
    }

    public void OnSpawnEnemyTimeout()
    {
        if (!_waveStarted) return;

        Random rng = new Random();
        string enemyPath = "";
        foreach (var enemy in Global.enemies)
        {
            int chance = (int)Math.Floor(rng.NextDouble() * 100);
            if (chance <= (enemy[1].ToInt() + (_currentWaveIndex + 1) * 3))
            {
                enemyPath = enemy[0];
                break;
            }
        }

        PackedScene enemyScene = GD.Load<PackedScene>(enemyPath);
        var newEnemy = enemyScene.Instantiate();
        GetNode<Path2D>("Enemies/Path2D").AddChild(newEnemy);

        enemiesLeft++;
        if (enemiesLeft <= _waves[_currentWaveIndex]) return;
        enemiesLeft = 0;
        _waveStarted = false;
        _currentWaveIndex++;
        GetNode<Timer>("%NextWaveDelay").Start();
    }

    public void OnNextWaveTimerTimeout()
    {
        if (_currentWaveIndex != 0) Global.Money += (_currentWaveIndex + 1) * 2 + 10;
        StartNextWave();
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

    public void OnButtonMenuPressed()
    {
        float offset = _menuIsOpen ? 1 : 0.65f;
        _menuIsOpen = !_menuIsOpen;

        Tween pos = CreateTween();
        pos.TweenProperty(GetNode<Control>("%ControlMenu"), "anchor_left", offset, .4f);
    }
}
