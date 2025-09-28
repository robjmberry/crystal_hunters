using Godot;
namespace CrystalHunters.Player.States;

public partial class FallState : AirState
{
    public override string Name => "Fall";
    public override void Enter()
    {
        // Play the "Hard 5" or "Comfortable 4" falling animation
        // Ensure your AnimationPlayer/AnimatedSprite has an animation named "JumpFall"
          Agent.AnimPlayer.Play("Fall");
    }
    
    public override void PhysicsUpdate(double delta)
    {
        // 1. Run Shared Air Logic (Gravity, Landing, Movement)
        base.PhysicsUpdate(delta);
    
        // 2. Coyote Time Jump
        // If the timer is still running, allow the player to jump late.
        // This is crucial for the "Comfortable" feel mentioned in your docs.
        if (!Agent.CoyoteTimer.IsStopped() && CheckJumpInput())
        {
           GD.Print("Used coyote time"); 
            ChangeState("Jump");
        }
    }
}