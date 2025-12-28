using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CrystalHunters.StateMachine;

// No longer a Node. It's a plain C# object.
public class StateMachine<T> where T : CharacterBody2D
{
    private readonly T _agent;
    private readonly Dictionary<string, State<T>> _states;
    private State<T> _currentState;

    // The constructor is now the main entry point for setup.
    public StateMachine(T agent, List<State<T>> states, State<T> initialState)
    {
        _agent = agent;
        _states = states.ToDictionary(s => s.Name);

        foreach (var s in states)
        {
            s.Init(agent, this);
        }
        
        _currentState = initialState;
        _currentState.Enter();
    }

    public void ChangeState(string stateName)
    {
        GD.Print($"Changing state to {stateName}.");
        if (!_states.TryGetValue(stateName, out var value))
        {
            GD.PrintErr($"State '{stateName}' not found in state machine.");
            return;
        }

        _currentState?.Exit();
        _currentState = value;
        _currentState.Enter();
    }

    // These methods are now called manually by the agent (PlayerController).
    public void PhysicsUpdate(double delta) => _currentState?.PhysicsUpdate(delta);
    public void HandleInput(InputEvent @event) => _currentState?.HandleInput(@event);
}


