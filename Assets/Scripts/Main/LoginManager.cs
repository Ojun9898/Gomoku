using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginManager : Singleton<LoginManager>
{

    private string currentUsername;
    private string userInfoFilepath = Path.Combine(Application.dataPath, "Data", "UserInfo.csv");

     public void AttemptLogin(string username, string password, Action<int> callback)
    {
        StartCoroutine(LoginCoroutine(username, password, callback));
    }

    private IEnumerator LoginCoroutine(string username, string password, Action<int> callback)
    {
        yield return new WaitForSeconds(0.5f); // 서버 요청 대기 시뮬레이션
        
        int result = CheckLogin(username, password);
        
        callback?.Invoke(result);
    }

    private int CheckLogin(string username, string password)
    {
        CheckFile();

        string[] lines = File.ReadAllLines(userInfoFilepath);

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');

            if (userData.Length == 5)
            {
                string storedUsername = userData[1].Trim();
                string storedPassword = userData[2].Trim();

                if (storedUsername == username && storedPassword == password)
                {
                    currentUsername = username;
                    return 1; // 로그인 성공
                }
            }
        }

        return 0; // 로그인 실패
    }    

    public List<string[]> GetAllUserInfo()
    {
        List<string[]> allUserInfo = new List<string[]>();

        CheckFile();

        string[] lines = File.ReadAllLines(userInfoFilepath);

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');

            if (userData.Length == 5)
            {
                allUserInfo.Add(userData);
            }
        }

        return allUserInfo;
    }
    public string[] GetUserInfo()
    {
        CheckFile();

        string[] lines = File.ReadAllLines(userInfoFilepath);

        foreach (string line in lines)
        {
            string[] userData = line.Split(',');

            if (userData.Length == 5)
            {
                string storedUsername = userData[1].Trim();
                if (storedUsername == currentUsername)
                {
                    return userData;
                }
            }
        }

        return null;
    }

    public string GetUsername()
    {
        return currentUsername;
    }

    public string CheckFile()
    {
        if (!File.Exists(userInfoFilepath))
            {
                Debug.LogError("로그인 파일이 없습니다.");
                return null;
            }
        return userInfoFilepath;
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
