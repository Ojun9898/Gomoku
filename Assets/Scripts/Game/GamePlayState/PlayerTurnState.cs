using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }
    public void Enter(Pc.Owner owner)
    {
        //모든 피스 공격 초기화
        GameManager.Instance.PieceSIni1t();
        //타일 on
        GameManager.Instance.SetTileClickEvent();
        //타이머 ON
        //랜즈룰 ON
        //카드 뽑기
        //코스트 증가
        Debug.Log(owner + "의 턴 입니다");
        Debug.Log("FirstDirectionState입니다");
    }

    public void Exit(Pc.Owner owner)
    {
        //ToDo : Ai 턴 생기면 활성화
        //GameManager.Instance.finishTurnButton.onClick.RemoveAllListeners();
        GameManager.Instance.SetTileClickEventOff();
        GameManager.Instance.SetFalseIsAleadySetPiece();
        Debug.Log("FirstDirectionState 나갔습니다");
    }
}

public class  RanzuLull
{
    
}