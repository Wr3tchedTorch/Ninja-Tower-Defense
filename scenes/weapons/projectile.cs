using Godot;
using System;

public partial class projectile : Area2D
{
    public Area2D Target;
    [Export]
    public int Speed = 200;
    [Export]
    public int Damage = 50;
    private bool _destroyed = false;

    public override void _PhysicsProcess(double delta)
    {
        if (!IsInstanceValid(Target) || Target.GetParent<Mob>().Health <= 0) 
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
            return;
        }

        if (!_destroyed)
        {
            LookAt(Target.GlobalPosition);
            GlobalPosition = GlobalPosition.MoveToward(Target.GlobalPosition, Speed * (float)delta);
        }
    }

    public void OnAreaEntered(Area2D area)
    {        
        if (_destroyed) return;

        area.GetParent<Mob>().Hit(Damage);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
        _destroyed = true;
    }    
}
