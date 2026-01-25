using System;
using Godot;

namespace  CrystalHunters.Health;

/// <summary>
/// Defines a contract for any entity that can receive damage.
/// </summary>
public interface ICombatDamageable
{
   /// <summary>
   /// Applies damage to the entity.
   /// </summary>
   /// <param name="damage">The damage details, including amount and position of the entity that caused the damage.</param>
   void TakeDamageFromCombat(int amountOfDamage, Vector2 sourceOfDamage);

  
}

public interface IHazardDamageable
{
   /// <summary>
   /// Applies damage to the entity.
   /// </summary>
   void TakeDamageFromHazard(int amountOfDamage);
}
