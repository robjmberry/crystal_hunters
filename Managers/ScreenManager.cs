using System.Threading.Tasks;
using CrystalHunters.Player;
using Godot;

namespace CrystalHunters.Managers;

public partial class ScreenManager : CanvasLayer
{
    private ColorRect _fadeRect;
    private RespawnManager _spawnManager;

    [Export] public float FadeDuration { get; set; } = 0.5f;

    public override void _Ready()
    {
        _fadeRect = GetNode<ColorRect>("ColorRect");
        _spawnManager = GetNode<RespawnManager>("/root/RespawnManager");

        // 1. Ensure screen starts transparent
        _fadeRect.Modulate = new Color(0, 0, 0, 0);
        
        // 2. Register ourselves with the Coordinator
        // (You need to add this method to SpawnManager, see below)
        _spawnManager.RegisterScreenManager(this);
    }

    // Task = "I promise to return when this job is done"
    public async Task FadeOut()
    {
        // Create a new Tween for this specific animation
        Tween tween = CreateTween();
        
        // Tween the "Alpha" (Transparency) to 1.0 (Fully Black)
        tween.TweenProperty(_fadeRect, "modulate:a", 1.0f, FadeDuration);
        
        // Pause here until the tween finishes
        await ToSignal(tween, Tween.SignalName.Finished);
    }

    public async Task FadeIn()
    {
        Tween tween = CreateTween();
        
        // Tween the "Alpha" to 0.0 (Invisible)
        tween.TweenProperty(_fadeRect, "modulate:a", 0.0f, FadeDuration);
        
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}