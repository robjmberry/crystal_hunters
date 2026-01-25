using Godot;

namespace CrystalHunters.Player.States;

public class RespawnState : PlayerState
{
    
    public override string Name => "Respawn";

    public override void Enter()
    {
        Agent.TriggerRespawn();
    }

    public override void HandleInput(InputEvent @event)
    {
        //Dont process input
    }
   
}