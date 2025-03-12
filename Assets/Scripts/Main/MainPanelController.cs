using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

public class MainPanelController : MonoBehaviour
{
    [SerializeField] private GameObject SettingPanel;
    [SerializeField] private GameObject BuyingPanel;
    [SerializeField] private Transform canvasTransform;

    private GameObject settingPanel;
    private RectTransform settingPanelRect;

    private GameObject buyingPanel;
    private RectTransform buyingPanelRect;

    public void OnClick1pButton()
    {
        Debug.Log("1p button clicked!");
    }

    public void OnClick2pButton()
    {
        Debug.Log("2p button clicked!");
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
        Debug.Log("Ranking button clicked!");
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
}
