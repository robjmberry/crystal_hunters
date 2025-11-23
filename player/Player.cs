using System;
using Godot;

namespace crystalhunters.player;

public partial class Player : CharacterBody2D
{
    [ExportCategory("Movement Metrics")]
    [Export] public float MoveSpeed { get; set; } = 400.0f;
    // Increase these! Try 1500 for Accel and 3000 for Braking with MoveToward
    [Export] public float Acceleration { get; set; } = 1200.0f; 
    [Export] public float Braking { get; set; } = 2000.0f; 

    [ExportCategory("Jump Metrics")]
    [Export] public float JumpHeight { get; set; } = 220.0f;
    [Export] public float TimeToApex { get; set; } = 0.4f;
    [Export] public float TimeToFall { get; set; } = 0.35f; // Fall faster than rise = heavy feel
    [Export] public float CoyoteTimeDuration { get; set; } = 0.1f;
    
    
    [ExportCategory("Visualisation")]
    [Export] public Node2D Visuals { get; set; }

    private float _gravityUp;
    private float _gravityDown;
    private float _jumpVelocity;
    private float _moveInput;
    private Timer _coyoteTimer;
    private bool _wasOnFloor;

    public override void _Ready()
    {
        _gravityUp = (2 * JumpHeight) / Mathf.Pow(TimeToApex, 2);
        _gravityDown = (2 * JumpHeight) / Mathf.Pow(TimeToFall, 2);
        _jumpVelocity = _gravityUp * TimeToApex * -1;

        _coyoteTimer = new Timer { WaitTime = CoyoteTimeDuration, OneShot = true };
        AddChild(_coyoteTimer);
        _wasOnFloor = IsOnFloor();
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float)delta;
        var newY = Velocity.Y;
        var isOnFloor = IsOnFloor();

        // 1. Coyote Time Logic
        if (_wasOnFloor && !isOnFloor && Velocity.Y >= 0) // Only start if falling
        {
            _coyoteTimer.Start();
        }

        // 2. Gravity Application
        if (!isOnFloor) 
            newY += Gravity() * dt;

        // 3. Jump Logic
        var canJump = isOnFloor || !_coyoteTimer.IsStopped();
        if (Input.IsActionJustPressed("jump") && canJump)
        {
            newY = _jumpVelocity;
            _coyoteTimer.Stop(); // Prevent double jumping from coyote time
        }

        // 4. Variable Jump Height (The Smooth Cut)
        if (Input.IsActionJustReleased("jump") && newY < 0)
            newY *= 0.5f; 

        // 5. Horizontal Movement (Snappy MoveToward)
        _moveInput = Input.GetAxis("move_left", "move_right");
        
        var targetSpeed = _moveInput * MoveSpeed;
        var accelRate = (_moveInput != 0) ? Acceleration : Braking;
        
        var newX = Mathf.MoveToward(Velocity.X, targetSpeed, accelRate * dt);

        // 6. Visual Flip
        if (_moveInput != 0 && Visuals != null)
        {
            var newScale = Visuals.Scale;
            newScale.X = _moveInput > 0 ? 1 : -1;
            Visuals.Scale = newScale;
        }

        Velocity = new Vector2(newX, newY);
        MoveAndSlide();
        _wasOnFloor = isOnFloor;
    }
    
    private float Gravity() => Velocity.Y < 0 ? _gravityUp : _gravityDown;
}