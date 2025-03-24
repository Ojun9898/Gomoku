using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyManager : Singleton<BuyManager>
{
    private string filePath = Path.Combine(Application.dataPath, "Data", "UserPurchases.csv");

    public string[] SetData()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.Combine(Application.dataPath, "Data", "UserPurchases.csv");
        }

        if (string.IsNullOrEmpty(filePath) || !Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Debug.LogError("Invalid file path: " + filePath);
            return null;
        }

        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Username,TotalCoin,BuyItems,UseItems\n");
        }

        string currentUsername = LoginManager.Instance.GetUsername();
        return GetBuyInfo(currentUsername);
    }

    public string[] GetBuyInfo(string currentUsername)
    {
        CheckFile();
        string[] lines = File.ReadAllLines(filePath);
        List<string> buyInfo = new List<string>();

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');

            if (userData.Length == 4 && userData[0].Trim() == currentUsername)
            {
                buyInfo.AddRange(userData);
                return buyInfo.ToArray();
            }
        }

        string newEntry = $"{currentUsername},0,,\n";
        File.AppendAllText(filePath, newEntry);

        return buyInfo.ToArray(); 
    }

    public void UpdateTotalCoin(int coin)
    {
        string currentUsername = LoginManager.Instance.GetUsername();
        string[] currentBuyInfo = GetBuyInfo(currentUsername); // GetBuyInfo 사용하여 구매 정보 가져오기

        if (currentBuyInfo == null || currentBuyInfo.Length != 4)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류");
            return;
        }

        int totalCoin = 0;
        if (!int.TryParse(currentBuyInfo[1], out totalCoin))
        {
            totalCoin = 0; // 만약 값이 비어있거나 잘못된 값이면 0으로 설정
        }

        totalCoin += coin; // 코인 추가
        currentBuyInfo[1] = totalCoin.ToString();

        // 전체 파일 내용을 갱신
        string[] lines = File.ReadAllLines(filePath);
        List<string> newLines = new List<string>();
        bool isUpdated = false;

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');
            if (userData.Length == 4 && userData[0].Trim() == currentUsername)
            {
                newLines.Add(string.Join(",", currentBuyInfo)); // 수정된 구매 정보 추가
                isUpdated = true;
            }
            else
            {
                newLines.Add(line); // 기존 데이터 그대로 추가
            }
        }

        if (!isUpdated)
        {
            newLines.Add(string.Join(",", currentBuyInfo)); // 새로 추가
        }

        File.WriteAllLines(filePath, newLines); // 파일에 저장
        MainManager.Instance.ShowErrorPanel("구매 완료되었습니다.");
    }



    public void UpdateBuyItems(string buyItems)
    {
    }

    public void UpdateUseItems(string useItems)
    {
    }


    private void CheckFile()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("구매 파일이 없습니다.");
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
