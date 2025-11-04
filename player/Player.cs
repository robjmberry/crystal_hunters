using System;
using Godot;

namespace crystalhunters.player;

public partial class Player : CharacterBody2D
{
    [Export] public float MoveSpeed { get; set; } = 401;
    [Export] public float Acceleration { get; set; } = 50;
    [Export] public float Breaking { get; set; } = 20;
    [Export] public float JumpHeight { get; set; } = 220;
    [Export] public float TimeToApex { get; set; } = 0.5f;
    [Export] public float TimeToFall { get; set; } = 0.4f;


    private float _gravityUp;
    private float _gravityDown;
    private float _jumpVelocity;
    private float _moveInput;

    public override void _Ready()
    {
        // Compute gravity for rising and falling
        
        _gravityUp = (2 * JumpHeight) / Mathf.Pow(TimeToApex, 2);
        _gravityDown = (2 * JumpHeight) / Mathf.Pow(TimeToFall, 2);
        _jumpVelocity = _gravityUp * TimeToApex *-1; //initial velocity to reach apex
    }

    public override void _PhysicsProcess(double delta)
    {
        var dt = (float) delta;
        var newY = Velocity.Y; 
        
        if (!IsOnFloor()) 
            newY +=  Gravity() * dt;    
        
        if (Input.IsActionJustPressed("jump") &&  IsOnFloor())
            newY = _jumpVelocity;
                
        _moveInput = Input.GetAxis("move_left", "move_right");
        
        var newVelocity = _moveInput != 0
            ? Mathf.Lerp(Velocity.X, _moveInput * MoveSpeed, Acceleration * dt)
            : Mathf.Lerp(Velocity.X, 0.0f, Breaking * dt);

        Velocity = new Vector2( newVelocity, newY);

        MoveAndSlide();
    }
    
    private float Gravity() => Velocity.Y < 0  ? _gravityUp : _gravityDown;
}