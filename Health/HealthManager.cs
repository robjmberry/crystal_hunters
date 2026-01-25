using Godot;
using System;

namespace CrystalHunters.Health;

public partial class HealthManager : Node
{
        [Signal] public delegate void HealthChangedEventHandler(int currentHealth, int maxHealth);
        [Signal] public delegate void DiedEventHandler();
        [Signal] public delegate void InvulnerabilityStartedEventHandler();
        [Signal] public delegate void InvulnerabilityEndedEventHandler();

        [Export] public int MaxHealth { get; set; } = 3;
        [Export] public float HurtCooldownTime { get; set; } = 1.0f;

        private int _currentHealth;
        private Timer _cooldownTimer;
        private bool _isInvulnerable = false;

        public override void _Ready()
        {
            _currentHealth = MaxHealth;

            // Setup the cooldown timer programmatically
            _cooldownTimer = new Timer();
            _cooldownTimer.WaitTime = HurtCooldownTime;
            _cooldownTimer.OneShot = true;
            _cooldownTimer.Timeout += OnCooldownTimeout;
            AddChild(_cooldownTimer);

            EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
        }

        public void Damage(int amount)
        {
            if (_isInvulnerable || amount <= 0) return;

            _currentHealth = Math.Clamp(_currentHealth - amount, 0, MaxHealth);
            EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);

            if (_currentHealth <= 0)
            {
                EmitSignal(SignalName.Died);
            }
            else
            {
                StartInvulnerability();
            }
        }

        private void StartInvulnerability()
        {
            _isInvulnerable = true;
            _cooldownTimer.Start();
            EmitSignal(SignalName.InvulnerabilityStarted);
        }

        private void OnCooldownTimeout()
        {
            _isInvulnerable = false;
            EmitSignal(SignalName.InvulnerabilityEnded);
        }

        public void Heal(int amount)
        {
            if (amount <= 0) return;
            _currentHealth = Math.Clamp(_currentHealth + amount, 0, MaxHealth);
            EmitSignal(SignalName.HealthChanged, _currentHealth, MaxHealth);
        }
    }
