using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MessageManager : Singleton<MessageManager>
{    
    [SerializeField] private Transform Canvas;
    [SerializeField] private GameObject MessagePanel;

    private GameObject messagePanel;
    private RectTransform messagePanelRect;

    public void ShowMessagePanel(string message)
    {
        if (messagePanel == null)
        {
            messagePanel = Instantiate(MessagePanel, Canvas);
            messagePanelRect = messagePanel.GetComponent<RectTransform>();
        }

        messagePanel.transform.SetAsLastSibling();
        messagePanel.GetComponentInChildren<TMP_Text>().text = message;
        // Fade in , Fade out
        CanvasGroup canvasGroup = messagePanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        canvasGroup.DOFade(1f,  0.3f).OnComplete(
            () => {canvasGroup.DOFade(0f,  0.3f);}
        );
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}
