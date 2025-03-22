using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDirectionState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Pc.Owner owner)
    {   //끝내기 연출
        Debug.Log("FinishDirectionState입니다");
    }

    public void Exit(Pc.Owner owner)
    {
        Debug.Log("FinishDirectionState 나갔습니다");
    }
}
