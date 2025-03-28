using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyManager : Singleton<BuyManager>
{
    private string streamingPath;
    private string persistentPath;

    private void Awake()
    {
        streamingPath = Path.Combine(Application.streamingAssetsPath, "UserPurchases.csv");
        persistentPath = Path.Combine(Application.persistentDataPath, "UserPurchases.csv");

        CopyFileIfNotExists();
    }

    private void CopyFileIfNotExists()
    {
        if (!File.Exists(persistentPath))
        {
            try
            {
                File.Copy(streamingPath, persistentPath, true);
                Debug.Log("파일이 StreamingAssets에서 persistentDataPath로 복사되었습니다.");
            }
            catch (Exception ex)
            {
                Debug.LogError("파일 복사 실패: " + ex.Message);
            }
        }
    }

    public string[] SetData()
    {
        if (!File.Exists(persistentPath))
        {
            Debug.LogError("파일이 존재하지 않습니다: " + persistentPath);
            return null;
        }

        string currentUsername = LoginManager.Instance.GetUsername();
        return GetBuyInfo(currentUsername);
    }

    public string[] GetBuyInfo(string currentUsername)
    {
        CheckFile();
        string[] lines = File.ReadAllLines(persistentPath);
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
        File.AppendAllText(persistentPath, newEntry);

        return new string[] { currentUsername, "0", "", "" };
    }

    public void UpdateTotalCoin(int coin)
    {
        string currentUsername = LoginManager.Instance.GetUsername();
        string[] currentBuyInfo = GetBuyInfo(currentUsername);

        if (currentBuyInfo == null || currentBuyInfo.Length != 4)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류");
            return;
        }

        int totalCoin = 0;
        if (!int.TryParse(currentBuyInfo[1], out totalCoin))
        {
            totalCoin = 0;
        }

        totalCoin += coin;
        currentBuyInfo[1] = totalCoin.ToString();

        string[] lines = File.ReadAllLines(persistentPath);
        List<string> newLines = new List<string>();
        bool isUpdated = false;

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');
            if (userData.Length == 4 && userData[0].Trim() == currentUsername)
            {
                newLines.Add(string.Join(",", currentBuyInfo));
                isUpdated = true;
            }
            else
            {
                newLines.Add(line);
            }
        }

        if (!isUpdated)
        {
            newLines.Add(string.Join(",", currentBuyInfo));
        }

        File.WriteAllLines(persistentPath, newLines);
        MainManager.Instance.ShowErrorPanel("구매 완료되었습니다.");
    }

    public void UpdateBuyItems(string buyItems)
    {
        // 구현 필요
    }

    public void UpdateUseItems(string useItems)
    {
        // 구현 필요
    }

    private void CheckFile()
    {
        if (!File.Exists(persistentPath))
        {
            Debug.LogError("구매 파일이 없습니다.");
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
