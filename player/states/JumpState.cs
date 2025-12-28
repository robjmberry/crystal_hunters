using Godot;

namespace CrystalHunters.Player.States;

public class JumpState : AirState
{
    public override string Name => "Jump";

    public override void Enter()
    {
        // Apply the jump velocity immediately
        var velocity = Agent.Velocity;
        velocity.Y = Agent.JumpVelocity;
        Agent.Velocity = velocity;
        Agent.MoveAndSlide();
        
        // Optional: Play jump sound or animation here
         Agent.AnimPlayer.Play("Jump");
    }

    public override void PhysicsUpdate(double delta)
    {
        base.PhysicsUpdate(delta);

        if (Input.IsActionJustReleased("jump"))
        {
            Agent.Velocity = new Vector2(Agent.Velocity.X, Agent.Velocity.Y * .5f);
        }
        
        // The jump impulse is instant, so we transition to the air/fall state immediately
        // or after one frame if you want to ensure the velocity is processed.
        // For now, we'll transition to "Fall" which handles all air movement.
        if (Agent.Velocity.Y > 0) ChangeState("Fall");
    }
}
