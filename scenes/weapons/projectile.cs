using Godot;

public partial class projectile : Area2D
{
    public Area2D Target;
    [Export]
    public int Speed = 200;
    [Export]
    public int Damage = 50;
    public bool Destroyed = false;
    public bool HasBeenThrowed = true;

    public override void _PhysicsProcess(double delta)
    {
        if (!HasBeenThrowed) return;

        if (!IsInstanceValid(Target) || Target.GetParent<Mob>().Health <= 0) 
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
            return;
        }

        if (!Destroyed)
        {
            LookAt(Target.GlobalPosition);
            GlobalPosition = GlobalPosition.MoveToward(Target.GlobalPosition, Speed * (float)delta);
        }
    }

    public virtual void OnAreaEntered(Area2D area)
    {
        if (!HasBeenThrowed) return;

        if (Destroyed) return;

        area.GetParent<Mob>().Hit(Damage);
        GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
        Destroyed = true;
    }
}
