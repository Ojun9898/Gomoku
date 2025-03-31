using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGamePanelController : MonoBehaviour
{
    public void OnClickYesButton()
    {
        GameManager.Instance.Notationcontroller?.SetCountZero();
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 

        SceneManager.LoadScene("Main");
    }

    public void OnClickNoButton()
    {
          this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

}

