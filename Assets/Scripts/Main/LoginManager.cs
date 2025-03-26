using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
    private string userInfoFilepath = Path.Combine(Application.dataPath, "Data", "UserInfo.csv");

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
            signinPanel.GetComponent<CanvasGroup>().DOFade(1f, fadeDuration);
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
            signupPanel.GetComponent<CanvasGroup>().DOFade(1f, fadeDuration);
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
            signinPanel.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration).OnComplete(() =>
            {
                signinPanel.SetActive(false);
            });
        }
    }

    public void CloseSignupPanel()
    {
        if (signupPanel != null && signupPanel.activeSelf)
        {
            signupPanel.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration).OnComplete(() =>
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
