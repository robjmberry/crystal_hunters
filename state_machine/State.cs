using Godot;

namespace CrystalHunters.StateMachine;

public abstract class State<T>
    where T : CharacterBody2D
{

    protected T Agent;
    protected StateMachine<T> StateMachine;
    
    public abstract string Name { get; }

    public void Init(T agent, StateMachine<T> stateMachine)
    {
        Agent = agent;
        StateMachine = stateMachine;
    }
    
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void PhysicsUpdate(double delta) { }
    public virtual void HandleInput(InputEvent @event) { }
    
    // Helper to change state safely
    protected void ChangeState(string newState)
    {
        StateMachine.ChangeState(newState);
    }
}