using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LoginManager : Singleton<LoginManager>
{
    [SerializeField] private GameObject SigninPanel;
    [SerializeField] private GameObject SignupPanel;

    [SerializeField] private GameObject ErrorPanel;
    [SerializeField] private Transform Canvas;

    private GameObject signinPanel;
    private GameObject signupPanel;
    private GameObject errorPanel;
    private RectTransform errorPanelRect;

    private float fadeDuration = 0.1f;
    private string currentUsername;
    private string userInfoFilepath;

    void Start()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "UserInfo.csv");
        string persistentPath = Path.Combine(Application.persistentDataPath, "UserInfo.csv");

        if (!File.Exists(persistentPath))
        {
            #if UNITY_ANDROID
            StartCoroutine(CopyFileFromStreamingAssets(streamingPath, persistentPath));

            #else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath, true);
            }

            #endif
        }

        userInfoFilepath = persistentPath;
    }

    private IEnumerator CopyFileFromStreamingAssets(string sourcePath, string destPath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(destPath, request.downloadHandler.data);
            }
            else
            {
                Debug.LogError("파일 복사 실패: " + request.error);
            }
        }
    }

    public void ShowSigninPanel()
    {
        if (signinPanel == null)
        {
            signinPanel = Instantiate(SigninPanel, Canvas);
        }

        if (!signinPanel.activeSelf)
        {
            signinPanel.SetActive(true);
            signinPanel.GetComponent<CanvasGroup>().DOFade(1, fadeDuration);
        }
    }

    public void ShowSignupPanel()
    {
        if (signupPanel == null)
        {
            signupPanel = Instantiate(SignupPanel, Canvas);
        }

        if (!signupPanel.activeSelf)
        {
            signupPanel.SetActive(true);
            signupPanel.GetComponent<CanvasGroup>().DOFade(1, fadeDuration);
        }
    }

    public void ShowErrorPanel(string message)
    {
        if (errorPanel == null)
        {
            errorPanel = Instantiate(ErrorPanel, Canvas);
            errorPanelRect = errorPanel.GetComponent<RectTransform>();
            errorPanelRect.anchoredPosition = new Vector2(-500f, 0f);
        }

        errorPanel.transform.SetAsLastSibling();
        errorPanel.GetComponentInChildren<TMP_Text>().text = message;
        errorPanel.SetActive(true);
        errorPanelRect.DOLocalMoveX(0f, 0.3f);
    }

    public void CloseSigninPanel()
    {
        if (signinPanel != null && signinPanel.activeSelf)
        {
            signinPanel.GetComponent<CanvasGroup>().DOFade(0, fadeDuration).OnComplete(() =>
            {
                signinPanel.SetActive(false);
            });
        }
    }

    public void CloseSignupPanel()
    {
        if (signupPanel != null && signupPanel.activeSelf)
        {
            signupPanel.GetComponent<CanvasGroup>().DOFade(0, fadeDuration).OnComplete(() =>
            {
                signupPanel.SetActive(false);
            });
        }
    }
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

            string storedUsername = userData[1].Trim();
            string storedPassword = userData[2].Trim();

            if (storedUsername == username && storedPassword == password)
            {
                currentUsername = username;
                return 1; // 로그인 성공
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
            allUserInfo.Add(userData);
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
            string storedUsername = userData[1].Trim();

            if (storedUsername == currentUsername)
            {
                return userData;
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

    /// <summary>
    /// 현재 로그인한 유저의 CSV 데이터에서 PlayerLevel과 LevelPoint를 갱신하는 메서드.
    /// levelPointDelta가 양수면 levelPoint에 더하고, 음수면 차감합니다.
    /// levelPoint가 3 이상이면 playerLevel을 +1하고 levelPoint에서 3을 빼며,
    /// levelPoint가 -3 이하이면 playerLevel을 -1하고 levelPoint에 3을 더합니다.
    /// </summary>
    public void UpdatePlayerLevelAndPoint(int levelPointDelta)
    {
        if (!File.Exists(userInfoFilepath))
        {
            Debug.LogError("CSV 파일이 존재하지 않습니다: " + userInfoFilepath);
            return;
        }

        // CSV 파일의 모든 라인 읽기
        string[] lines = File.ReadAllLines(userInfoFilepath);
        List<string> newLines = new List<string>();

        // 첫 번째 라인은 헤더이므로 그대로 추가
        if (lines.Length > 0)
        {
            newLines.Add(lines[0]);
        }

        bool updated = false;

        // 각 데이터 행을 순회 (헤더 제외)
        for (int i = 1; i < lines.Length; i++)
        {
            string[] tokens = lines[i].Split(',');
            // CSV의 형식: Date, Username, Password, Nickname, Score, PlayerLevel, LevelPoint
            if (tokens.Length < 7)
            {
                newLines.Add(lines[i]);
                continue;
            }

            // 현재 로그인한 유저의 데이터 찾기 (username 비교)
            if (tokens[1].Trim() == currentUsername)
            {
                int currentPlayerLevel = 0;
                int currentLevelPoint = 0;
                int.TryParse(tokens[5].Trim(), out currentPlayerLevel);
                int.TryParse(tokens[6].Trim(), out currentLevelPoint);

                // levelPoint에 변화량 적용
                currentLevelPoint += levelPointDelta;

                // levelPoint가 3 이상이면 playerLevel 업, 3씩 차감
                while (currentLevelPoint >= 3)
                {
                    currentPlayerLevel++;
                    currentLevelPoint -= 3;
                }

                // levelPoint가 -3 이하이면 playerLevel 다운, 3씩 보정
                while (currentLevelPoint <= -3)
                {
                    currentPlayerLevel--;
                    currentLevelPoint += 3;
                }

                // 갱신된 값을 문자열로 다시 할당
                tokens[5] = currentPlayerLevel.ToString();
                tokens[6] = currentLevelPoint.ToString();

                updated = true;
            }
            // 수정(또는 그대로 유지)된 토큰들을 쉼표로 결합
            newLines.Add(string.Join(",", tokens));
        }

        if (updated)
        {
            // CSV 파일에 변경된 내용 저장
            File.WriteAllLines(userInfoFilepath, newLines.ToArray());
            Debug.Log("플레이어 레벨과 포인트가 업데이트되었습니다.");
        }
        else
        {
            Debug.LogWarning("업데이트할 사용자를 찾을 수 없습니다: " + currentUsername);
        }
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DOTween.KillAll();

        if (scene.name == "Login")
        {
            Canvas = GameObject.Find("Canvas")?.transform;

            if (Canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다!");
                return;
            }

            ShowSigninPanel();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
