using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static Pc;

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

    private IState currentState;
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public void Run(Pc.Owner owner)
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
        states.Add(state.GetType(), state);
    }

    public void ChangeState<T>(Pc.Owner owner) where T : IState
    {
        ChangeState(typeof(T), owner);
    }

    private void ChangeState(Type stateType, Pc.Owner owner)
    {
        currentState?.Exit(owner);

        if (!states.TryGetValue(stateType, out currentState)) return;
        currentState?.Enter(owner);
    }
}