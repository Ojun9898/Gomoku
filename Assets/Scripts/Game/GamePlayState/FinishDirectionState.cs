using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishDirectionState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {   //끝내기 연출
        //owner에는 우승자 정보가 들어감 여기서 요걸로 판별하거나 아님 먼저 하거나
        //패널하나  열어서  보여주는 것도 ㄱㅊ을듯?
        GameManager.Instance.gamePanelController.StopTimer();

        // 플레이어 승리 여부에 따라 levelPoint를 업데이트
        if (owner == Piece.Owner.PLAYER_A)
        {
            // 플레이어 A가 이기면 CSV 파일의 로그인 정보에 대해 levelPoint를 +1 증가시킴
            LoginManager.Instance.UpdatePlayerLevelAndPoint(1);
        }
        else if (owner == Piece.Owner.PLAYER_B)
        {
            // 플레이어 B가 이기면 levelPoint를 -1 감소시킴
            LoginManager.Instance.UpdatePlayerLevelAndPoint(-1);
        }

        // 기보 저장
        NotationManager.Instance.SaveData(GameManager.Instance.playerInfo[3]);

        // GameOverPanel 띄우기
        MainManager.Instance.ShowGameOverPanel(owner);
        
        Debug.Log("FinishDirectionState입니다");
    }

    public void Exit(Piece.Owner owner)
    {
        Debug.Log("FinishDirectionState 나갔습니다");
    }
}
