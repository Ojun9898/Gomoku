using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTurnState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Pc.Owner owner)
    {   //렌주룰
        GameManager.Instance._rullManager.UpdateForbiddenMoves(owner);
    
        //모든 피스 공격 초기화
        GameManager.Instance.PieceSIni1t();
        //타일 on
        GameManager.Instance.SetTileClickEvent();
        //타이머 ON
        GameManager.Instance.GamePanelController.StartTimer();
        //카드 뽑기
        //코스트 증가

        Debug.Log(owner + "의 턴 입니다");
    }

    public void Exit(Pc.Owner owner)
    {
        //렌주룰 풀기
        GameManager.Instance._rullManager.DeleteForviddensOnMap();
        //ToDo : Ai 턴 생기면 활성화
        //GameManager.Instance.finishTurnButton.onClick.RemoveAllListeners();
        GameManager.Instance.SetTileClickEventOff();
        GameManager.Instance.SetFalseIsAlreadySetPiece();
        GameManager.Instance.AllTileClickCountSetZero();
        Debug.Log("PlayerTurnState 나갔습니다");
    }
}

public class  RanzuLull
{
    
}