using Godot;

public partial class area_damage : projectile
{

    public override void OnAreaEntered(Area2D area)
    {
        if (!HasBeenThrowed) return;

        area.GetParent<Mob>().Hit(Damage);
        if (GetNode<camera>("/root/Level/%Camera2D").ShakeStrength < 1) GetNode<camera>("/root/Level/%Camera2D").ApplyShake(10);
        if (!GetNode<AnimationPlayer>("AnimationPlayer").IsPlaying()) GetNode<AnimationPlayer>("AnimationPlayer").Play("break");
        Destroyed = true;
    }
}
