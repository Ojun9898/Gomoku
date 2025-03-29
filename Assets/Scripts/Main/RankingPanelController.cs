using System;
using System.Collections.Generic;
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
        RankingRankerInfo = new List<string[]>(AllUserInfo); // 깊은 복사

        // 랭킹 정렬
        for (int i = 0; i < RankingRankerInfo.Count; i++)
        {
            for (int j = i + 1; j < RankingRankerInfo.Count; j++)
            {
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

        // Rank 설정 (동점자 처리)
        int currentRank = 1;
        for (int i = 0; i < RankingRankerInfo.Count; i++)
        {
            if (i > 0)
            {
                int prevScore = ParseScore(RankingRankerInfo[i - 1][4]);
                int currScore = ParseScore(RankingRankerInfo[i][4]);

                if (prevScore != currScore)
                {
                    currentRank = i + 1;
                }
            }

            // 배열 크기 확인 후 Rank 추가

            var tempArray = RankingRankerInfo[i];
             Array.Resize(ref tempArray, 6);
            RankingRankerInfo[i] = tempArray;

            RankingRankerInfo[i][5] = "Rank " + currentRank;
        }
    }

    private int ParseScore(string score)
    {
        if (int.TryParse(score, out int parsedScore))
        {
            return parsedScore;
        }
        return 0; // 유효하지 않으면 0 처리
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

            rankItem.GetChild(0).GetComponent<TMP_Text>().text = RankingRankerInfo[i][3]; // 닉네임
            rankItem.GetChild(1).GetComponent<TMP_Text>().text = RankingRankerInfo[i][4]; // 점수
            rankItem.GetChild(2).GetComponent<TMP_Text>().text = RankingRankerInfo[i][5]; // 랭크
        }
    }

    public void SettingPlayerPanel()
    {
        string[] updatedPlayerInfo = new string[Math.Max(playerInfo.Length, 6)];
        Array.Copy(playerInfo, updatedPlayerInfo, playerInfo.Length);

        for (int a = 0; a < RankingRankerInfo.Count; a++)
        {
            if (!string.IsNullOrEmpty(playerInfo[1]) && playerInfo[1] == RankingRankerInfo[a][1]) // Username 확인
            {
                updatedPlayerInfo[5] = RankingRankerInfo[a].Length > 5 ? RankingRankerInfo[a][5] : "N/A"; // Rank 저장
                break;
            }
        }

        PlayerPanel.transform.GetChild(0).GetComponent<TMP_Text>().text = updatedPlayerInfo[3] ?? "N/A"; // 닉네임
        PlayerPanel.transform.GetChild(1).GetComponent<TMP_Text>().text = updatedPlayerInfo[4] ?? "0"; // 점수
        PlayerPanel.transform.GetChild(2).GetComponent<TMP_Text>().text = updatedPlayerInfo[5]; // 랭크
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
