using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private GameObject TooltipPanel;
    [SerializeField] private Transform Canvas;
    private GameObject tooltipPanel;
    private TMP_Text tooltipText;

    void Start()
    {
        InitializeTooltip();
    }

    void InitializeTooltip()
    {
        if (tooltipPanel != null) return; // 이미 생성된 경우 중복 생성 방지

        Canvas = GameObject.Find("Canvas")?.transform;

        if (Canvas != null && TooltipPanel != null)
        {
            tooltipPanel = Instantiate(TooltipPanel, Canvas);
            tooltipPanel.SetActive(false);
            tooltipText = tooltipPanel.transform.GetChild(0).GetComponent<TMP_Text>();
        }
    }


    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            tooltipPanel.transform.position = mousePos + new Vector2(150f, -100f);
        }
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipPanel.SetActive(true);
    }

    public void CloseTooltip()
    {
        tooltipPanel.SetActive(false);
    }
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeTooltip();
    }
}
