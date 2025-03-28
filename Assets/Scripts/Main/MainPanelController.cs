using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject BuyingPanel;
    [SerializeField] private GameObject RankingPanel;
    [SerializeField] private GameObject ProfilePanel;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private GameObject FadePanel;

    private GameObject settingPanel;
    private RectTransform settingPanelRect;

    private GameObject buyingPanel;
    private RectTransform buyingPanelRect;

    private GameObject rankingPanel;
    private RectTransform rankingPanelRect;
    private GameObject profilePanel;
    private RectTransform profilePanelRect;
    
    void Awake()
    {
        FindCanvas();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void FindCanvas()
    {
        // Canvas를 다시 찾음
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            canvasTransform = canvasObj.transform;
        }
        else
        {
            Debug.LogError("Canvas를 찾을 수 없습니다!");
        }
    }
    public void OnClickPlayButton()
    {
        FadePanel = Instantiate(FadePanel, canvasTransform);

        FadePanel.GetComponent<CanvasGroup>().alpha = 0f;

        FadePanel.GetComponent<CanvasGroup>().DOFade(1f, 1.5f).OnComplete(() =>
        {
            SceneManager.LoadScene("Game");
        });
    }

    public void OnClickExitButton()
    {
        if (Application.isEditor)
        {
            UnityEditor.EditorApplication.isPlaying = false;
        }
        
        else Application.Quit();
    }

    public void OnClickRankingButton() 
    {
        if (rankingPanel == null)
        {
            rankingPanel = Instantiate(RankingPanel, canvasTransform);
            rankingPanelRect = rankingPanel.GetComponent<RectTransform>();
            rankingPanelRect.anchoredPosition = new Vector2(-500f, 0f); // 초기 위치 설정
        }
        else
        {
            rankingPanel.SetActive(true);
        }

        rankingPanelRect.DOLocalMoveX(0f, 0.3f);
    }

    public void OnClickSettingButton()
    {
        if (settingPanel == null)
        {
            settingPanel = Instantiate(SettingPanel, canvasTransform);
            settingPanelRect = settingPanel.GetComponent<RectTransform>();
            settingPanelRect.anchoredPosition = new Vector2(-500f, 0f); // 초기 위치 설정
        }
        else
        {
            settingPanel.SetActive(true);
        }

        settingPanelRect.DOLocalMoveX(0f, 0.3f);
    }

    public void OnClickBuyingButton() 
    {
        if (buyingPanel == null)
        {
            buyingPanel = Instantiate(BuyingPanel, canvasTransform);
            buyingPanelRect = buyingPanel.GetComponent<RectTransform>();
            buyingPanelRect.anchoredPosition = new Vector2(-500f, 0f); // 초기 위치 설정
        }
        else
        {
            buyingPanel.SetActive(true);
        }

        buyingPanelRect.DOLocalMoveX(0f, 0.3f); 
    }

    public void OnClickProfileButton()
    {
       if(profilePanel == null) 
       {
            profilePanel = Instantiate(ProfilePanel, canvasTransform);
            profilePanelRect = profilePanel.GetComponent<RectTransform>();
            profilePanelRect.anchoredPosition = new Vector2(-500f, 515f); // 초기 위치 설정
        }
        else
        {
            profilePanel.SetActive(true);
        }

        profilePanelRect.DOLocalMoveX(0f, 0.3f);
       
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindCanvas();
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
