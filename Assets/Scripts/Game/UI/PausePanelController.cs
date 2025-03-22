using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PausePanelController : MonoBehaviour
{
    private CanvasGroup canvasGroup;

    public void OnClickYesButton()
    {
        //GameManager.Instance.ResumeGame();
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(0f, 0.5f).OnComplete(() => //투명
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }
    public void OnClickNoButton()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.DOFade(0f, 0.5f).OnComplete(() => //투명
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        });
    }
}
