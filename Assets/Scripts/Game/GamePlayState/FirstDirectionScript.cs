using DG.Tweening;
using UnityEngine;

public class FirstDirectionScript : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {

        // 시작 연출
        Debug.Log("FirstDirectionState입니다");

        // 5초 후에 AI턴으로 넘어갈지 플레이어 턴으로 넘어갈지 고름
        DOVirtual.DelayedCall(1, () =>
        {
            if (owner == Piece.Owner.PLAYER_A)
            {
                Fsm.ChangeState<PlayerTurnState>(owner);
            }
            else {
                Fsm.ChangeState<AITurnState>(owner);
            }
        });        
    }

    public void Exit(Piece.Owner owner)
    {
        Debug.Log("FirstDirectionState 나갔습니다");
    }
}
