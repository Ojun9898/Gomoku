using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePanelController : MonoBehaviour
{
    [SerializeField] private GameObject timerObject;
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private Transform canvasTransform;
    private Timer timer;
    private CanvasGroup canvasGroup;
    private GameObject pausePanel;

    private void Awake()
    {
        timer = timerObject.GetComponent<Timer>();
    }

    public void OnClickPauseButton()
    {
        //GameManager.Instance.PauseGame();
        ShowPausePanel();
    }

    public void OnClickTurnButton()
    {
        Debug.Log("Turn Button Clicked");
        //GameManager.Instance.OnButtonClickFinishMyTurn();
    }

    public void StartTimer() {
        timer.StartTimer();
    }

    public void StopTimer() {
        timer.StopTimer();
    }
    public void OnTimerEnd()
    {
        if (GameManager.Instance.GetIsAlReadySetPiece() == false) {
            //Todo: 피스 생성 메소드 시작

        }
        GameManager.Instance.OnButtonClickFinishMyTurn();
    } 



    public void ShowPausePanel()
    {
        pausePanel = Instantiate(PausePanel, canvasTransform);

        canvasGroup = pausePanel.GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }
}
