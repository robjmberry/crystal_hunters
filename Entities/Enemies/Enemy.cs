using Godot;
using System;
using CrystalHunters.Health;
using CrystalHunters.Player;

namespace CrystalHunters.Entities.Enemies;

public partial class Enemy : Area2D
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
       if (body is not ICombatDamageable damageable) return;
       
       GD.Print($"Player hit! Dealing {DamageAmount} damage.");
       damageable.TakeDamageFromCombat(DamageAmount, Vector2.Zero);
   }
}
