using DG.Tweening;
using UnityEngine;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private Transform canvasTransform;
    private Timer timer;
    private CanvasGroup canvasGroup;
    private GameObject pausePanel;

    public void InitTimer()
    {
        timer = timerObject.GetComponent<Timer>();
    }
    public void OnClickPauseButton()
    {
       // GameManager.Instance.PauseGame();
        ShowPausePanel();
    }

    public void OnClickTurnButton()
    {
        GameManager.Instance.OnButtonClickFinishMyTurn();
    }

    public void StartTimer() {
        timer.SetSliderColor();
        timer.StartTimer();
    }

    public void StopTimer() {
        timer.SetSliderColor();
        timer.StopTimer();
    }
   
    public void ShowPausePanel()
    {
        if (pausePanel == null) { 
            pausePanel = Instantiate(PausePanel, canvasTransform);
        }
        pausePanel.SetActive(true);
        canvasGroup = pausePanel.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(1, 0.5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }
}
