using UnityEngine;

public class PlayerTurnState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {   
 
        //렌주룰
        GameManager.Instance.ruleManager.UpdateForbiddenMoves(owner);
    
        //모든 피스 공격 초기화
        GameManager.Instance.PiecesInit();
        //타일 on
        GameManager.Instance.SetTileClickEvent();
        //타이머 ON
        GameManager.Instance.gamePanelController.StartTimer();
        //카드 뽑기
        //코스트 증가

        //턴 텍스트 설정
        string playerType = owner.ToString();
        TurnPanelController tp = FindObjectOfType<TurnPanelController>();
        tp.ShowTurnText(playerType);
    }

    public void Exit(Piece.Owner owner)
    {
        //렌주룰 풀기
        GameManager.Instance.ruleManager.DeleteForviddensOnMap();
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