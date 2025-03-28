using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EndGamePanelController : MonoBehaviour
{
    public void OnClickYesButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
        
        MainManager.Instance.EndGame();
    }

    public void OnClickNoButton()
    {
          this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

}

