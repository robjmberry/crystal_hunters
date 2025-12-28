using CrystalHunters.StateMachine;
using Godot;

namespace CrystalHunters.Player.States;
// This class inherits the generic machinery but locks T to PlayerController
public abstract class PlayerState : State<PlayerController>
{
    protected bool CheckJumpInput()
    {
        return Input.IsActionJustPressed("jump");
    }
    
    protected float GetMoveInput()
    {
        return Input.GetAxis("move_left", "move_right");
    }
}