using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishDirectionState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
<<<<<<< Updated upstream
    {   //끝내기 연출
        //owner에는 우승자 정보가 들어감 여기서 요걸로 판별하거나 아님 먼저 하거나
        //패널하나  열어서  보여주는 것도 ㄱㅊ을듯?
        GameManager.Instance.gamePanelController.StopTimer();
        MessageManager.Instance.ShowMessagePanel($"{owner}의 승리입니다!");
        StartCoroutine(WaitFinishGame());
=======
    {
        GameManager.Instance.gamePanelController.StopTimer();
        // GameOverPanel 띄우기
        MainManager.Instance.ShowGameOverPanel(owner);
>>>>>>> Stashed changes
        Debug.Log("FinishDirectionState입니다");
    }

    public void Exit(Piece.Owner owner)
    {
        Debug.Log("FinishDirectionState 나갔습니다");
    }
<<<<<<< Updated upstream

    IEnumerator WaitFinishGame()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene("Main");
    }
=======
>>>>>>> Stashed changes
}
