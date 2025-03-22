using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FirstDirectionScript : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Pc.Owner owner)
    {

        // 시작 연출
        Debug.Log("FirstDirectionState입니다");

        // 5초 후에 AI턴으로 넘어갈지 플레이어 턴으로 넘어갈지 고름
        DOVirtual.DelayedCall(1, () =>
        {
            if (owner == Pc.Owner.PLAYER_A)
            {
                Fsm.ChangeState<PlayerTurnState>(owner);
            }
            else {
                Fsm.ChangeState<AITurnState>(owner);
            }
        });        
    }

    public void Exit(Pc.Owner owner)
    {
        Debug.Log("FirstDirectionState 나갔습니다");
    }
}
