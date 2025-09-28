namespace CrystalHunters.Player.States;

// Inherits from PlayerState, so it has access to 'Agent' (PlayerController)
public abstract class GroundedState : PlayerState
{
    public override void PhysicsUpdate(double delta)
    {
        // 1. Shared Slope Physics
        // Even on the ground, we apply gravity to keep the CharacterBody2D 
        // snapped to slopes during movement.
        Agent.ApplyGravity(delta);

        // 2. Shared Jump Logic
        // We define this ONCE here. Both Idle and Run now have jumping.
        if (CheckJumpInput()) 
        {
            ChangeState("Jump");
            return;
        }

        // 3. Shared Fall Logic
        // If we walk off a ledge, we must fall.
        if (!Agent.IsOnFloor())
        {
            Agent.CoyoteTimer.Start();
            ChangeState("Fall");
            return;
        }
 
    }
}