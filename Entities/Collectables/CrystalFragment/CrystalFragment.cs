using System;
using Godot;

// Namespace matches the folder structure for organization
namespace CrystalHunters.Entites.Collectables.CrystalFragment;

/// <summary>
/// Game logic for the crystal fragments that the player collects.
/// </summary>
public partial class CrystalFragment : Area2D
{
    [Export] public int ScoreValue { get; set; } = 100;
    
    [Export] public Sprite2D Visuals { get; set; }
    
    [Export] public GpuParticles2D Particles2D { get; set; }
    
    [Export] public AudioStreamPlayer2D AudioStreamPlayer2D { get; set; }

    // Define a static event so the UI or Game Manager can listen without hard references
    public static event Action<int> OnCrystalCollected;

    public override void _Ready()
    {
        // Connect the BodyEntered signal dynamically
        BodyEntered += OnBodyEntered;

        // Visuals: Start a simple idle animation (bobbing/rotating)
        StartIdleAnimation();
        
        RandomiseGlimmerShader();
    }

    /// <summary>
    /// Ensure each crystal has a different time to glimmer at.
    /// </summary>
    private void RandomiseGlimmerShader()
    {
        // We must duplicate the material so changing the offset on this crystal 
        // doesn't change it for EVERY crystal in the game.
        if (Visuals.Material is not ShaderMaterial mat) return;
        
        var uniqueMat = (ShaderMaterial)mat.Duplicate();
        Visuals.Material = uniqueMat;

        // Randomise the start time between 0 and 5 seconds
        uniqueMat.SetShaderParameter("time_offset", GD.Randf() * 5.0f);
    }

    private void OnBodyEntered(Node2D body)
    {
        // Although Physics Layers filter most collisions, we explicitly check
        // if the body is the Player to be safe.
        // Assuming your player class is named 'KaraController' or uses a 'Player' group.
        if (body is CharacterBody2D) 
        {
            Collect();
        }
    }

    private void Collect()
    {
        // Disable collision to prevent double-triggering while the animation plays
        SetDeferred(Area2D.PropertyName.Monitoring, false);
        
        // Notify the game system (Score Manager)
        OnCrystalCollected?.Invoke(ScoreValue);
       
        // Play the sound
        AudioStreamPlayer2D.PitchScale = (float)GD.RandRange(0.9, 1.1);
        AudioStreamPlayer2D.Play();

        //Start the particles
        Particles2D.Emitting = true;


        // Visual cleanup: Tween scale to 0 before deleting
        var tween = CreateTween();
        tween.TweenProperty(this, "scale", Vector2.Zero, 0.1f)
             .SetTrans(Tween.TransitionType.Back)
             .SetEase(Tween.EaseType.In);
        
        // 3. Wait for particles to finish before destroying the object
        // The lifetime is 1.0s, so we wait slightly longer to be safe.
        GetTree().CreateTimer(1.2f).Timeout += QueueFree; 
    }

    private void StartIdleAnimation()
    {
        // Simple code-based bobbing animation instead of an AnimationPlayer
        var tween = CreateTween().SetLoops();
        tween.TweenProperty(this, "position:y", Position.Y - 5.0f, 1.0f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(this, "position:y", Position.Y, 1.0f)
             .SetTrans(Tween.TransitionType.Sine)
             .SetEase(Tween.EaseType.InOut);
    }
}