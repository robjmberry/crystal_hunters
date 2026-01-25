using CrystalHunters.Health;
using Godot;

namespace CrystalHunters.Entities.Enemies;

public partial class Hazard : Area2D
{
    [Export]
    public int DamageAmount { get; set; }
   
  
    public override void _Ready()
    {
        // Subscribe to the signal that fires when a body enters this Area2D
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is not IHazardDamageable damageable) return;
       
        GD.Print($"Player hit! Dealing {DamageAmount} damage.");
        damageable.TakeDamageFromHazard(DamageAmount);
    }
}