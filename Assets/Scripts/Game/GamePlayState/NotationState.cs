using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NotationState :MonoBehaviour, IState
{
    public StateMachine Fsm { get; set ; }

    public List<bool> AICosts = new List<bool>();
    public List<bool> PlayerCosts = new List<bool>();
    public void Enter(Piece.Owner owner)
    {
        if (owner == Piece.Owner.PLAYER_B)
        {
            // 코스트 최대 카운트 증가
            if (AICosts.Count < 10)
            {
                AICosts.Add(true);
            }
            // 코스트 활성화
            for (int i = 0; i < AICosts.Count; i++)
            {
                AICosts[i] = true;
            }
            Debug.Log("현재 코스트: " + AICosts.Count);
            GameManager.Instance.cp.SetCost(AICosts);

            GameManager.Instance.Costs = AICosts;

        }
        else {
            // 코스트 최대 카운트 증가
            if (PlayerCosts.Count < 10)
            {
                PlayerCosts.Add(true);
            }
            // 코스트 활성화
            for (int i = 0; i < PlayerCosts.Count; i++)
            {
                PlayerCosts[i] = true;
            }
            Debug.Log("현재 코스트: " + PlayerCosts.Count);
            GameManager.Instance.cp.SetCost(PlayerCosts);

            GameManager.Instance.Costs = PlayerCosts;
        }


        //렌즈룰
        GameManager.Instance.ruleManager.UpdateForbiddenMoves(owner);
        //타일 On
        GameManager.Instance.SetTileClickEvent();
        //모든 피스 공격 초기화
        GameManager.Instance.PiecesInit();

        //턴 텍스트 설정
        TurnPanelController tp = FindObjectOfType<TurnPanelController>();
        tp.ShowTurnText(owner);

        GameManager.Instance.Notationcontroller.WatchPlay();
    }

    public void Exit(Piece.Owner owner)
    {
    }
}
