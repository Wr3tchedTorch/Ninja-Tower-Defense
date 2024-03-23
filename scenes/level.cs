using System;
using Godot;

public partial class level : Node2D
{
    private int _health = 100;
    private bool _gameoverScreenFaded = false;
    private bool _menuIsOpen = false;
    private bool _closedMenuAfterBuying = false;
    private int enemiesLeft = 0;
    public int EnemiesAlive = 0;
    private int[] _waves = { 10, 20, 30, 40, 50 };
    private bool _waveStarted = false;
    private int _currentWaveIndex = 0;
    private bool _levelFinished = false;
    private bool _levelFinishedScreenFaded = false;

    private void transitionTweenFinished() => GetNode<ColorRect>("%TransitionRect").Visible = false;

    public override void _Ready()
    {
        GetNode<ColorRect>("%TransitionRect").Visible = true;
        Tween transitionTween = CreateTween();
        transitionTween.TweenProperty(GetNode<ColorRect>("%TransitionRect"), "color", new Color(Colors.Black, 0f), 1);
        Callable transitionTweenFinished = new Callable(this, "transitionTweenFinished");
        transitionTween.Connect("finished", transitionTweenFinished);

        Global.Money = 10;

        GetNode<ColorRect>("%RedBackground").Visible = false;

        GetNode<Timer>("%NextWaveDelay").Start();
        GetNode<Label>("%WaveLabel").Text = "Wave: " + (_currentWaveIndex + 1);
        _waveStarted = false;
    }

    public override void _Process(double delta)
    {
        if (_levelFinishedScreenFaded) return;

        HandleWaveState();

        if (Global.IsBuilding && !_closedMenuAfterBuying)
        {
            _closedMenuAfterBuying = true;
            OnButtonMenuPressed();
        }

        GetNode<Label>("%MoneyLabel").Text = "Money: " + Global.Money;

        if (!Global.IsBuilding) _closedMenuAfterBuying = false;

        if (Input.IsActionPressed("reset-game")) GetTree().ReloadCurrentScene();

        if (GetNode<ProgressBar>("%HealthBar").Value > 0 || _gameoverScreenFaded) return;
        Gameover();
    }

    public void TakeDamage(int damage) {
        if (_health <= 0) return;
        _health -= damage;
        if (GetNode<camera>("%Camera2D").ShakeStrength < 1) GetNode<camera>("%Camera2D").ApplyShake(5);
        
        GetNode<ColorRect>("%RedBackground").Visible = true;
        Tween redBgTween = GetTree().CreateTween();        
        redBgTween.TweenProperty(GetNode<ColorRect>("%RedBackground"), "modulate", new Color(Colors.White, .1f), 0.1);
        redBgTween.TweenProperty(GetNode<ColorRect>("%RedBackground"), "modulate", new Color(Colors.White, 0f), 0.2);       

        Callable callable = new Callable(this, "redBgTweenFinished");
        redBgTween.Connect("finished", callable);
    }

    private void redBgTweenFinished() {
        if (_health > 0) GetNode<ColorRect>("%RedBackground").Visible = false;
    }

    private void HandleWaveState()
    {
        if (EnemiesAlive > 0) return;

        if (_currentWaveIndex >= _waves.Length)
        {
            _levelFinished = true;
            _levelFinishedScreenFaded = true;
            LevelFinished();
            return;
        }

        if (GetNode<Timer>("%NextWaveDelay").TimeLeft <= 0 && !_waveStarted)
        {
            _currentWaveIndex++;
            if (_currentWaveIndex >= _waves.Length) return;

            Global.Money += (_currentWaveIndex + 1) * 2 + 10;
            GetNode<Timer>("%NextWaveDelay").Start();
            StartNextWave();
        }
    }

    private void ShowNewScreen(string backgroundPath, string firstLabel, string secondLabel, string tweenFinishFunctionName, Color textColor)
    {
        Tween tweenLevelFinishedScreen = GetTree().CreateTween();

        GetNode<ColorRect>(backgroundPath).Visible = true;
        tweenLevelFinishedScreen.TweenProperty(GetNode<ColorRect>(backgroundPath), "modulate", new Color(Colors.White, 1f), 1);
        tweenLevelFinishedScreen.TweenProperty(GetNode<Label>(firstLabel), "modulate", new Color(textColor, 1), 1);
        tweenLevelFinishedScreen.TweenProperty(GetNode<Label>(secondLabel), "modulate", new Color(textColor, 1), 1);

        Callable callable = new Callable(this, tweenFinishFunctionName);
        tweenLevelFinishedScreen.Connect("finished", callable);
    }

    private void LevelFinished()
    {
        ShowNewScreen("%BlueBackground", "%LevelFinishedLabel1", "%LevelFinishedLabel2", "OnLevelFinishedTweenFinished", Colors.Green);
        _levelFinishedScreenFaded = true;
    }

    private void Gameover()
    {
        ShowNewScreen("%RedBackground", "%GameoverLabel1", "%GameoverLabel2", "OnGameOverTweenFinished", Colors.Red);
        _gameoverScreenFaded = true;
    }

    private void OnGameOverTweenFinished() => BlinkText("%GameoverLabel3");
    private void OnLevelFinishedTweenFinished() => BlinkText("%LevelFinishedLabel3");

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
    }

    private void BlinkText(string filePath)
    {
        Tween blinkText = CreateTween().SetLoops();

        blinkText.TweenProperty(GetNode<Label>(filePath), "modulate", new Color(Colors.White, 0), .7f);
        blinkText.TweenProperty(GetNode<Label>(filePath), "modulate", new Color(Colors.White, 1), .7f);
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
    }

    public void OnNextWaveTimerTimeout()
    {
        GD.Print("Next wave");
        _waveStarted = true;
    }

    public void OnFinishLineAreaEntered(Area2D area)
    {
        if (area.GetParent() is Mob mob)
        {
            TakeDamage(mob.Damage);
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
