using System.Collections.Generic;
using CrystalHunters.Player.States;
using CrystalHunters.StateMachine;
using Godot;

namespace CrystalHunters.Player;

public partial class PlayerController : CharacterBody2D
{
    [ExportCategory("Movement Metrics")]
     [Export] public float MoveSpeed { get; set; } = 500.0f;
     // Increase these! Try 1500 for Accel and 3000 for Braking with MoveToward
     [Export] public float Acceleration { get; set; } = 1200.0f; 
     [Export] public float Braking { get; set; } = 2000.0f; 

     [ExportCategory("Jump Metrics")]
     [Export] public float JumpHeight { get; set; } = 192.0f;
     [Export] public float TimeToApex { get; set; } = 0.3f;
     [Export] public float TimeToFall { get; set; } = 0.28f; // Fall faster than rise = heavy feel
     [Export] public float CoyoteTimeDuration { get; set; } = 0.1f;


    [ExportCategory("Components")]
    [Export] public Sprite2D Visuals { get; set; }
    [Export] public AnimationPlayer AnimPlayer { get; set; }
    
    [Export] public CollisionShape2D FacingRightCollision { get; set; }
    [Export] public CollisionShape2D FacingLeftCollision { get; set; }

    
    // --- THIS IS THE PART YOU WERE MISSING ---
    // We use public properties with 'get' so States can read them, 
    // but only this script can write to them (private set).
    public float GravityUp { get; private set; }
    public float GravityDown { get; private set; }
    public float JumpVelocity { get; private set; }
    
    public Timer CoyoteTimer { get; private set; }
    public Timer JumpBufferTimer { get; private set; }

    private StateMachine<PlayerController> _stateMachine;
    
    
    private float _gravityUp;
     private float _gravityDown;
  

    public override void _Ready()
    {
        RecalculatePhysics();
        
        CoyoteTimer = new Timer { WaitTime = CoyoteTimeDuration, OneShot = true };
        JumpBufferTimer = new Timer { WaitTime = 0.1f, OneShot = true };
        AddChild(CoyoteTimer);
        AddChild((JumpBufferTimer));

        var states = new List<State<PlayerController>> {  new IdleState(), new FallState(), new RunState(), new JumpState(), new LandingState()  };
        _stateMachine = new StateMachine<PlayerController>(this, states, states[0]);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        _stateMachine.PhysicsUpdate(delta);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
        _stateMachine.HandleInput(@event);
    }

    private void RecalculatePhysics()
    {

        _gravityUp = (2 * JumpHeight) / Mathf.Pow(TimeToApex, 2);
         _gravityDown = (2 * JumpHeight) / Mathf.Pow(TimeToFall, 2);
         JumpVelocity = _gravityUp * TimeToApex * -1;
    }

    // Helper methods for States to use
    public void ApplyGravity(double delta)
    {
        var gravity = Gravity();
        var velocity = Velocity;
        velocity.Y += gravity * (float)delta;
        Velocity = velocity;
    }

    public void MoveX(float targetSpeed, float accel)
    {
        float direction = Input.GetAxis("move_left", "move_right");

        if (direction != 0f)
        {
            Visuals.FlipH = direction < 0f;
            FacingRightCollision.Disabled = direction < 0f;
            FacingLeftCollision.Disabled = direction > 0f;
        }

        var velocity = Velocity;
        velocity.X = Mathf.MoveToward(Velocity.X, targetSpeed, accel);
        Velocity = velocity;

    }
    
    private float Gravity() => Velocity.Y < 0 ? _gravityUp : _gravityDown;
}