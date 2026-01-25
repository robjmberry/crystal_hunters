namespace CrystalHunters.Player.States;

// Inherits from PlayerState, so it has access to 'Agent' (PlayerController)
public abstract class GroundedState : PlayerState
{
    private const double SafeTimeOnGround = 0.2;
    private double _timeOnGround;
    
    public override void PhysicsUpdate(double delta)
    {
        // 0. Update Safe Location
        // Since we are grounded, this is a valid place to respawn.
        Agent.SafeLocation = Agent.GlobalPosition;

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
            _timeOnGround = 0;
            Agent.CoyoteTimer.Start();
            ChangeState("Fall");
            
        }
        else
        {
            _timeOnGround += delta;
            if (_timeOnGround > SafeTimeOnGround)
            {
                Agent.SafeLocation = Agent.GlobalPosition;
                _timeOnGround = 0;
            }

        }
 
    }
}
