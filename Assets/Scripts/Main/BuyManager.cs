using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuyManager : Singleton<BuyManager>
{
    private string streamingPath;
    private string persistentPath;
    string currentUsername;
    private string[] buyInfo;
    private string BuyItem;
    private string UseItem;
    List<string> availableItems = new List<string>();
    Dictionary<string, int> itemCounts = new Dictionary<string, int>();

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

    public string[] SetCSVFileData()
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

    public int GetCoins()
    {
        string[] buyInfo = SetCSVFileData();

        if (buyInfo == null)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류입니다");
            return 0;
        }

        return int.TryParse(buyInfo[1], out int coins) ? coins : 0;
    }

    public void UpdateTotalCoin(int coin)
    {
        currentUsername = LoginManager.Instance.GetUsername();
        string[] currentBuyInfo = GetBuyInfo(currentUsername);

        if (currentBuyInfo == null || currentBuyInfo.Length != 4)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류입니다");
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
        MainManager.Instance.ShowErrorPanel("구매 완료되었습니다");
    }

    public void UpdateBuyItems(string buyItem)
    {
        currentUsername = LoginManager.Instance.GetUsername();
        string[] currentBuyInfo = GetBuyInfo(currentUsername);

        if (currentBuyInfo == null)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류");
            return;
        }

        // 기존 구매 아이템 정보 업데이트
        if (string.IsNullOrWhiteSpace(currentBuyInfo[2]))
        {
            currentBuyInfo[2] = buyItem;
        }
        else
        {
            currentBuyInfo[2] += "." + buyItem; // 기존 아이템 목록에 '.'으로 추가
        }

        // 기존 파일에서 해당 유저 데이터만 수정하여 다시 저장
        string[] lines = File.ReadAllLines(persistentPath);
        List<string> newLines = new List<string>();

        foreach (string line in lines)
        {
            string[] userData = line.Split(','); // 기존 정보는 ','로 구분
            if (userData[0].Trim() == currentUsername)
            {
                userData[2] = currentBuyInfo[2]; // 기존 데이터에서 구매 아이템 목록만 수정
                newLines.Add(string.Join(",", userData));
            }
            else
            {
                newLines.Add(line);
            }
        }

        File.WriteAllLines(persistentPath, newLines);
        Debug.Log("BuyItem 업데이트: " + currentBuyInfo[2]);
    }


    public void UpdateUseItems(string useItem)
    {
        currentUsername = LoginManager.Instance.GetUsername();
        string[] currentBuyInfo = GetBuyInfo(currentUsername);

        if (currentBuyInfo == null)
        {
            MainManager.Instance.ShowErrorPanel("구매 정보 오류");
            return;
        }

        // 기존 사용 아이템 정보 업데이트
        if (string.IsNullOrWhiteSpace(currentBuyInfo[3]))
        {
            currentBuyInfo[3] = useItem;
        }
        else
        {
            currentBuyInfo[3] += "." + useItem; // 기존 아이템 목록에 '.'으로 추가
        }

        // 기존 파일에서 해당 유저 데이터만 수정하여 다시 저장
        string[] lines = File.ReadAllLines(persistentPath);
        List<string> newLines = new List<string>();

        foreach (string line in lines)
        {
            string[] userData = line.Split(','); // 기존 정보는 ','로 구분
            if (userData[0].Trim() == currentUsername)
            {
                userData[3] = currentBuyInfo[3]; // 기존 데이터에서 사용 아이템 목록만 수정
                newLines.Add(string.Join(",", userData));
            }
            else
            {
                newLines.Add(line);
            }
        }

        File.WriteAllLines(persistentPath, newLines);
        Debug.Log("UseItem 업데이트: " + currentBuyInfo[3]);
    }


    public void SetItemData()
    {
        // 이전 데이터를 초기화
        availableItems.Clear();
        itemCounts.Clear();

        // CSV 파일에서 최신 구매 정보를 가져옴
        buyInfo = SetCSVFileData();

        // 구매 아이템과 사용 아이템 문자열 가져오기
        BuyItem = buyInfo[2]; // 예: "timer.timer.timer"
        UseItem = buyInfo[3]; // 예: "timer"

        // 공백이나 빈 항목 제거 옵션 포함해서 분리
        List<string> buyItemList = new List<string>(BuyItem.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries));
        List<string> useItemList = new List<string>(UseItem.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries));

        // 사용한 아이템 하나씩 제거 (멀티셋 차집합)
        foreach (string used in useItemList)
        {
            // buyItemList에서 한 번만 제거
            if (buyItemList.Contains(used))
            {
                buyItemList.Remove(used);
            }
        }

        // 남은 아이템들이 availableItems가 됨
        availableItems = buyItemList;

        // availableItems의 각 아이템 개수를 카운트 (선택 사항)
        foreach (string item in availableItems)
        {
            if (!string.IsNullOrEmpty(item))
            {
                if (itemCounts.ContainsKey(item))
                {
                    itemCounts[item]++;
                }
                else
                {
                    itemCounts[item] = 1;
                }
            }
        }
    }


    public List<string> GetAvailableItemInfo()
    {
        SetItemData();
        return availableItems;
    }

    public int GetItemCount(string item)
    {
        SetItemData();
        return itemCounts[item];
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
