using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishDirectionState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {
        GameManager.Instance.gamePanelController.StopTimer();
        // GameOverPanel 띄우기
        MainManager.Instance.ShowGameOverPanel(owner);
        Debug.Log("FinishDirectionState입니다");
    }

    public void Exit(Piece.Owner owner)
    {
        Debug.Log("FinishDirectionState 나갔습니다");
    }
}
