using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
>>>>>>> Stashed changes

public class EndGamePanelController : MonoBehaviour
{
    public void OnClickYesButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
        
<<<<<<< Updated upstream
        MainManager.Instance.EndGame();
=======
        SceneManager.LoadScene("Main");
>>>>>>> Stashed changes
    }

    public void OnClickNoButton()
    {
          this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

}

