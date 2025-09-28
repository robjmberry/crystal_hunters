using Godot;

namespace CrystalHunters.Player.States;

public partial class LandingState : GroundedState
{
    public override string Name => "Land";

    // How long the landing animation plays before auto-switching to Idle
    private float _landingTime = 0.15f; 
    private float _timer;

    public override void Enter()
    {
        // Ensure you have a "Land" animation in your AnimationPlayer
        Agent.AnimPlayer.Play("Land");
        _timer = _landingTime;
    }

    public override void PhysicsUpdate(double delta)
    {
        if (Agent.JumpBufferTimer.IsStopped() == false)
        {
            GD.Print("Jump buffer");
            ChangeState("Jump");
        }

        // 1. Shared Ground Logic (Gravity, Jump, Fall)
        // This allows "Bunny Hopping" because GroundedState checks for Jump!
        base.PhysicsUpdate(delta);

        // 2. Friction
        // Apply braking to stop sliding from momentum
        Agent.MoveX(0, Agent.Braking * (float)delta);
        Agent.MoveAndSlide();

        // 3. Animation Canceling (Snappy Controls)
        // If the player tries to move, skip the rest of the landing lag and Run.
        if (GetMoveInput() != 0)
        {
            ChangeState("Run");
            return;
        }

        // 4. Timer -> Idle
        _timer -= (float)delta;
        if (_timer <= 0)
        {
            ChangeState("Idle");
        }
    }
}