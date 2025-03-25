using UnityEngine;

public class AITurnState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }
    public void Enter(Piece.Owner owner)
    {
        //렌즈룰
        GameManager.Instance.ruleManager.UpdateForbiddenMoves(owner);

        //모든 피스 공격 초기화
        GameManager.Instance.PiecesInit();
        GameManager.Instance.SetTileClickEvent();
        GameManager.Instance.gamePanelController.StartTimer();

        //돌 두기 ai
        //돌 공격 ai
        //더이상 공격할 수 없거나(코스트 부족),공격할 게 없을 때 (모든 piece범위에 적 없음 or 공격 남은 말이 없음)
        //턴 변경

        Debug.Log(owner + "의 턴 입니다");
    }

    public void Exit(Piece.Owner owner)
    {
        GameManager.Instance.ruleManager.DeleteForviddensOnMap();
        GameManager.Instance.SetTileClickEventOff();
        GameManager.Instance.SetFalseIsAlreadySetPiece();
        GameManager.Instance.AllTileClickCountSetZero();
        Debug.Log("AITurnState 나갔습니다");
    }
}