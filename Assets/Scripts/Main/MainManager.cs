using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainManager : Singleton<MainManager>
{
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private Transform Canvas;
    [SerializeField] private GameObject ErrorPanel;
    [SerializeField] private GameObject LogoutPanel;
    [SerializeField] private GameObject EndGamePanel;

    private GameObject mainPanel;
    private GameObject errorPanel;
    private RectTransform errorPanelRect;
    private GameObject logoutPanel;
    private RectTransform logoutPanelRect;
    private GameObject endGamePanel;
    private RectTransform endGamePanelRect;
    private float fadeDuration = 0.1f;

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void CloseMainPanel()
    {
        if (MainPanel.activeSelf)
        {
            MainPanel.GetComponent<CanvasGroup>().DOFade(0f, fadeDuration).OnComplete(() =>
            {
                MainPanel.SetActive(false);
            });
        }
    }

    public void ShowMainPanel()
    {
        if (SceneManager.GetActiveScene().name == "Login" || 
            SceneManager.GetActiveScene().name == "Game")
        {
            return;
        }

        if (mainPanel == null)
        {
            mainPanel = Instantiate(MainPanel, Canvas);
        }

        if (!mainPanel.activeSelf)
        {
            mainPanel.SetActive(true);
            mainPanel.GetComponent<CanvasGroup>().DOFade(1f, fadeDuration);
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

    public void ShowLogoutPanel()
    {
        if (logoutPanel == null)
        {
            logoutPanel = Instantiate(LogoutPanel, Canvas);
            logoutPanelRect = logoutPanel.GetComponent<RectTransform>();
            logoutPanelRect.anchoredPosition = new Vector2(-500f, 0f);
        }

        logoutPanel.transform.SetAsLastSibling();
        logoutPanel.SetActive(true);
        logoutPanelRect.DOLocalMoveX(0f, 0.3f);
    }

    public void ShowEndGamePanel()
    {
        if (endGamePanel == null)
        {
            endGamePanel = Instantiate(EndGamePanel, Canvas);
            endGamePanelRect = endGamePanel.GetComponent<RectTransform>();
            endGamePanelRect.anchoredPosition = new Vector2(-500f, 0f);
        }

        endGamePanel.transform.SetAsLastSibling();
        endGamePanel.SetActive(true);
        endGamePanelRect.DOLocalMoveX(0f, 0.3f);
    }

    public void Logout()
    {
        CloseMainPanel();
        SceneManager.LoadScene("Login");
    }

    public void EndGame()
    {
        SceneManager.LoadScene("Main");
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Main" || scene.name == "Game")
        {
            DOTween.KillAll();
            Canvas = GameObject.Find("Canvas")?.transform;

            if (Canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다!");
                return;
            }

            ShowMainPanel();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
