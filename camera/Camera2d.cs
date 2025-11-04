using Godot;

namespace crystalhunters.camera;

public partial class Camera2d : Camera2D
{
    [Export] public Node2D Target { get; set; }
    
    public override void _PhysicsProcess(double delta)
    {
        // Smoothly interpolate toward the target
        var targetPos = GlobalPosition.Lerp(Target.GlobalPosition, 0.2f);

        // Round to nearest pixel to avoid subpixel blur
        GlobalPosition = new Vector2(
            Mathf.Round(targetPos.X),
            Mathf.Round(targetPos.Y)
        );
    }
}