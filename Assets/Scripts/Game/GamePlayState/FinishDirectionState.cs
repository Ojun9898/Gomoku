using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FinishDirectionState : MonoBehaviour, IState
{
    public StateMachine Fsm { get; set; }

    public void Enter(Piece.Owner owner)
    {   //끝내기 연출
        //owner에는 우승자 정보가 들어감 여기서 요걸로 판별하거나 아님 먼저 하거나
        //패널하나  열어서  보여주는 것도 ㄱㅊ을듯?
        var userInfoFilepath = GameManager.Instance.userInfoFilepath;
        LoginManager.Instance.CheckFile();
        
        string[] lines = File.ReadAllLines(userInfoFilepath);
        
        foreach (var line in lines)
        {
            string[] userData = line.Split(',');

            if (userData.Length == 7)
            {
                string storedPlayerLevel = userData[5].Trim();
                string storedPlayerPoint = userData[6].Trim();

                if (owner == Piece.Owner.PLAYER_A)
                {
                    
                }
            }
        }
        
        Debug.Log(owner + "의 승리입니다.");
        Debug.Log("FinishDirectionState입니다");
    }

    public void Exit(Piece.Owner owner)
    {
        Debug.Log("FinishDirectionState 나갔습니다");
    }
}
