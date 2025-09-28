namespace CrystalHunters.Player.States;

public partial class RunState : GroundedState
{
    public override string Name => "Run";

    public override void Enter()
    {
        Agent.AnimPlayer.Play("Run");
    }

    public override void PhysicsUpdate(double delta)
    {
        // 1. Run the Parent Logic first (Gravity, Jump, Ledge check)
        base.PhysicsUpdate(delta);

        // 2. Handle Movement
        float input = GetMoveInput();

        if (input == 0)
        {
            ChangeState("Idle");
            return;
        }

        // Apply Velocity
        float targetSpeed = input * Agent.MoveSpeed;
        Agent.MoveX(targetSpeed, Agent.Acceleration * (float)delta);
        Agent.MoveAndSlide();
    }
}