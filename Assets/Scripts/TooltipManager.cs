using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TooltipManager : Singleton<TooltipManager>
{
    [SerializeField] private GameObject TooltipPanel;
    [SerializeField] private Transform Canvas;
    private GameObject tooltipPanel;
    private RectTransform tooltipPanelRect;
    private TMP_Text tooltipText;

    // 텍스트와 패널에 적용할 패딩 값 (원하는 만큼 조절)
    private Vector2 padding = new Vector2(20f, 15f);

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
            tooltipPanelRect = tooltipPanel.GetComponent<RectTransform>();
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
        // 텍스트 설정 및 한 줄 표시를 위한 설정
        tooltipText.text = message;
        tooltipText.enableWordWrapping = false;                     // 단어 래핑 비활성화
        tooltipText.overflowMode = TextOverflowModes.Overflow;       // 텍스트 오버플로우 허용

        // 텍스트 업데이트 후 크기 조정
        AdjustTooltipSize();

        tooltipPanel.SetActive(true);
    }

    private void AdjustTooltipSize()
    {
        if (tooltipText == null || tooltipPanelRect == null)
            return;

        // ForceMeshUpdate를 통해 최신 텍스트 레이아웃 계산
        tooltipText.ForceMeshUpdate();

        // 텍스트의 권장 너비와 높이 계산 (한 줄 텍스트이므로 preferredHeight는 한 줄 높이)
        float preferredWidth = tooltipText.preferredWidth;
        float preferredHeight = tooltipText.preferredHeight;

        // 텍스트 크기에 패딩을 추가하여 툴팁 패널의 크기를 조정
        tooltipPanelRect.sizeDelta = new Vector2(preferredWidth + padding.x, preferredHeight + padding.y);
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
