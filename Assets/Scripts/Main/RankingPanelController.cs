using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class RankingPanelController : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPanel;
    [SerializeField] private GameObject RankerPanel;
    [SerializeField] private TMP_Text UpdateTimeText;

    List<string[]> AllUserInfo;
    List<string[]> RankingRankerInfo = new List<string[]>();

    private string[] playerInfo;


    void Start()
    {
        AllUserInfo = LoginManager.Instance.GetAllUserInfo();
        playerInfo = LoginManager.Instance.GetUserInfo();

        SortingRanking();
        UpdateTime();
        SettingPlayerPanel();
        SettingRankerPanel();
    }

    public void SortingRanking()
    {
        RankingRankerInfo = AllUserInfo;

        // 랭킹 정보 정렬
        for (int i = 0; i < RankingRankerInfo.Count; i++)
        {
            for (int j = i + 1; j < RankingRankerInfo.Count; j++)
            {
                // 점수 순으로 내림차순 정렬 (빈 데이터는 0 점으로 처리)
                int scoreI = ParseScore(RankingRankerInfo[i][4]); 
                int scoreJ = ParseScore(RankingRankerInfo[j][4]);

                if (scoreI < scoreJ)
                {
                    string[] temp = RankingRankerInfo[i];
                    RankingRankerInfo[i] = RankingRankerInfo[j];
                    RankingRankerInfo[j] = temp;
                }
            }
        }

        // Rank 생성 (동점자 처리)
        int currentRank = 1;
        for (int i = 0; i < RankingRankerInfo.Count; i++)
        {
            if (i > 0)
            {
                int prevScore = ParseScore(RankingRankerInfo[i - 1][4]);
                int currScore = ParseScore(RankingRankerInfo[i][4]);

                // 같은 점수면 랭크를 동일하게 설정
                if (prevScore != currScore)
                {
                    currentRank = i + 1;
                }
            }
            
            // Rank 설정
            RankingRankerInfo[i][5] = "rank " + currentRank;
        }
    }


    // 점수를 안전하게 변환하는 함수
    private int ParseScore(string score)
    {
        int parsedScore = 0;

        // 점수가 유효한 숫자 형식인지 확인하고, 아니면 기본값 0을 반환
        if (!int.TryParse(score, out parsedScore))
        {
            parsedScore = 0;  // 유효하지 않으면 0으로 처리
        }
        return parsedScore;
    }


    public void SettingRankerPanel()
    {
        int maxRank = Mathf.Min(10, RankingRankerInfo.Count);

        for (int i = 0; i < maxRank; i++)
        {
            Transform rankItem = RankerPanel.transform.GetChild(i);
            
            if (rankItem == null)
            {
                continue;
            }

            if (RankingRankerInfo[i].Length > 3)
                rankItem.GetChild(0).GetComponent<TMP_Text>().text = RankingRankerInfo[i][3]; // nickname

            if (RankingRankerInfo[i].Length > 4)
                rankItem.GetChild(1).GetComponent<TMP_Text>().text = RankingRankerInfo[i][4]; // score
            
            if (RankingRankerInfo[i].Length > 5)
                rankItem.GetChild(2).GetComponent<TMP_Text>().text = RankingRankerInfo[i][5]; // rank
            
        }
    }

    public void SettingPlayerPanel()
    {
        string[] updatedPlayerInfo = new string[Math.Max(playerInfo.Length, 7)];

        for (int i = 0; i < playerInfo.Length; i++)
        {
            updatedPlayerInfo[i] = playerInfo[i];
        }

        for (int a = 0; a < RankingRankerInfo.Count; a++)
        {
            if (!string.IsNullOrEmpty(playerInfo[1]) && playerInfo[1] == RankingRankerInfo[a][1]) // username
            {
                updatedPlayerInfo[6] = RankingRankerInfo[a].Length > 5 ? RankingRankerInfo[a][5] : "N/A"; // rank
                break;
            }
        }

        PlayerPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = updatedPlayerInfo[3] ?? "N/A"; // nickname
        PlayerPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = updatedPlayerInfo[4] ?? "0"; // score
        PlayerPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = updatedPlayerInfo[6]; // rank
    }



        
    public void UpdateTime()
    {
        UpdateTimeText.text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        UpdateTimeText.ForceMeshUpdate();
    }

    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
