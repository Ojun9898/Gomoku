using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum StaterType
{
    None,
    PlayGame,
    Max
}


public static class StateFactory
{
    public static List<IState> CreateStates(this StateMachine stateMachine, StaterType staterType)
    {
        List<IState> states = new List<IState>();

        switch (staterType)
        {
            case StaterType.PlayGame:
                {
                    states.Add(stateMachine.AddComponent<FirstDirectionScript>());
                    states.Add(stateMachine.AddComponent<PlayerTurnState>());
                    states.Add(stateMachine.AddComponent<FinishDirectionState>());
                    states.Add(stateMachine.AddComponent<AITurnState>());
                }
                break;
        }

        return states;
    }
}

public class StateMachine : MonoBehaviour
{
    [SerializeField] private string defaultState;

    private IState _currentState;
    private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();

    public void Run(Piece.Owner owner)
    {
        List<IState> states = this.CreateStates(StaterType.PlayGame);
        foreach (var state in states)
        {
            AddState(state);
        }

        ChangeState(Type.GetType(defaultState), owner);
    }

    public void AddState(IState state)
    {
        state.Fsm = this;
        _states.Add(state.GetType(), state);
    }

    public void ChangeState<T>(Piece.Owner owner) where T : IState
    {
        ChangeState(typeof(T), owner);
    }

    private void ChangeState(Type stateType, Piece.Owner owner)
    {
        _currentState?.Exit(owner);

        if (!_states.TryGetValue(stateType, out _currentState)) return;
        _currentState?.Enter(owner);
    }
}