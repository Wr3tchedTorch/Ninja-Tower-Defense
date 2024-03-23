using Godot;

public partial class area_damage : projectile
{

    public override void OnAreaEntered(Area2D area)
    {
        if (!HasBeenThrowed) return;

        area.GetParent<Mob>().Hit(Damage);

        if (!GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying()) GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
        Destroyed = true;
    }
}
