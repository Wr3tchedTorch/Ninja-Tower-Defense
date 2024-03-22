using Godot;

public partial class tower_button : Control
{
    [Export]
    public string TargetTowerScenePath;
    [Export]
    public int MoneyCost;
    private bool _hovering = false;
    private bool _brought = false;

    public override void _Process(double delta)
    {
        if (Global.Money >= MoneyCost && _brought)
        {
            PackedScene towerScene = GD.Load<PackedScene>(TargetTowerScenePath);
            tower newTower = towerScene.Instantiate<tower>();

            GetNode<Node2D>("/root/Level/Towers").AddChild(newTower);
            Global.Money -= MoneyCost;
        }

        _brought = false;

        if (_hovering == true && Input.IsActionJustPressed("mb-left")) _brought = true;        
    }

    public void OnMouseEntered()
    {
        _hovering = true;
        Tween hoverTween = CreateTween();
        hoverTween.TweenProperty(GetNode<ColorRect>("HoverRect"), "color", new Color(Colors.Black, 0.4f), .2f);
    }

    public void OnMouseExited()
    {
        _hovering = false;
        Tween exitHoverTween = CreateTween();
        exitHoverTween.TweenProperty(GetNode<ColorRect>("HoverRect"), "color", new Color(Colors.Black, 0f), .2f);
    }
}
