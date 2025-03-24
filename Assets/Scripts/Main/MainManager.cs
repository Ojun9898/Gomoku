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
    [SerializeField] private GameObject SigninPanel;
    [SerializeField] private GameObject SignupPanel;
    [SerializeField] private Transform Canvas;
    [SerializeField] private GameObject ErrorPanel;

    private GameObject errorPanel;
    private RectTransform errorPanelRect;
    private GameObject signinPanel;
    private GameObject signupPanel;
    private float fadeDuration = 0.1f;

   void Start()
    {
        ShowSigninPanel();
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

    public void ShowMainPanel()
    {
        if (!MainPanel.activeSelf)
        {
            MainPanel.SetActive(true);
            MainPanel.GetComponent<CanvasGroup>().DOFade(1, fadeDuration);
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

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
