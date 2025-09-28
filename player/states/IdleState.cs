using Godot;

namespace CrystalHunters.Player.States;

public partial class IdleState : GroundedState
{
    public override  string Name => "Idle";
    public override void Enter()
    {
        // Play the idle animation immediately upon entering this state
        Agent.AnimPlayer.Play("Idle");
        
        // Optional: If you want 'snappy' controls where letting go of the stick
        // stops you instantly, you could set Velocity.X to 0 here.
        // For now, we will let friction handle it below for a smoother feel.
    }

    public override void PhysicsUpdate(double delta)
    {
        // 1. IMPORTANT: Run the GroundedState logic first!
        // This checks if we pressed Jump or walked off a ledge.
        base.PhysicsUpdate(delta);

        // 2. Friction / Braking
        // We tell the controller to move toward 0 speed using the Braking force.
        Agent.MoveX(0, Agent.Braking * (float)delta);
        
        // Apply the velocity (Standard Godot MoveAndSlide)
        Agent.MoveAndSlide();

        // 3. Transition to Run
        // If the player presses Left or Right, switch to RunState.
        if (GetMoveInput() != 0)
        {
            ChangeState("Run");
        }
    }
}