using System;
using System.Threading.Tasks;
using Godot;

namespace CrystalHunters.Managers;

public partial class RespawnManager : Node
{
    private ScreenManager _screenManager;
    
    public async Task StartRespawn(Node2D who, Vector2 location, Action afterRespawn)
    {
        await _screenManager.FadeOut();
        
        await ToSignal(GetTree().CreateTimer(0.3f), SceneTreeTimer.SignalName.Timeout);
        who.GlobalPosition = location;

        await _screenManager.FadeIn();
        
        afterRespawn();
    }

    public void RegisterScreenManager(ScreenManager screenManager)
    {
        _screenManager = screenManager;
    }
}