using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static Piece;

public class FirstDirectionScript : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {

        // 시작 연출
        Debug.Log("FirstDirectionState입니다");

        StartCoroutine(StartGame(owner));
    }
    IEnumerator StartGame(Piece.Owner owner) {
      
        yield return new WaitForSeconds(1);
        if (owner == Piece.Owner.PLAYER_A)
        {
            Fsm.ChangeState<PlayerTurnState>(owner);
        }
        else
        {
            Fsm.ChangeState<AITurnState>(owner);
        }
    }

    public void Exit(Piece.Owner owner)
    {
        var blackPanel = GameManager.Instance.BlackPanel.GetComponent<Image>();
        blackPanel.DOFade(0, 0.5F).OnComplete(() => {
            GameManager.Instance.BlackPanel.SetActive(false);
        });
        Debug.Log("FirstDirectionState 나갔습니다");
    }
}
