
namespace CrystalHunters.Player.States;

public abstract  class AirState : PlayerState
{
    public override void PhysicsUpdate(double delta)
    {
        // 1. Gravity Logic
        // We check velocity to decide which gravity to apply (Up vs Down)
        // This supports the "Variable Jump Height" physics from your Design Doc.
        Agent.ApplyGravity(delta);
    
        // 2. Air Movement (Air Control)
        var input = GetMoveInput();
        if (input != 0)
        {
            var targetSpeed = input * Agent.MoveSpeed;
            // You might want different acceleration in air (e.g. Agent.Acceleration * 0.5f)
            // For now, we use the standard acceleration for snappy controls.
            Agent.MoveX(targetSpeed, Agent.Acceleration * (float)delta);
        }
        else
        {
            // Air Drag: When no input, slow down slightly or preserve momentum?
            // Using 'Braking' gives you tight control even in air.
            Agent.MoveX(0, Agent.Braking * (float)delta);
        }
    
        Agent.MoveAndSlide();
        
        if (CheckJumpInput())
            Agent.JumpBufferTimer.Start();
        
        // 3. Landing Check (Transition to Ground)
        if (Agent.IsOnFloor())
        {
            ChangeState("Land");
        }
    }
}