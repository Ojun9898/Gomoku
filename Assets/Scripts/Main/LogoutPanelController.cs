using DG.Tweening;
using UnityEngine;
<<<<<<< Updated upstream
=======
using UnityEngine.SceneManagement;
>>>>>>> Stashed changes

public class LogoutPanelController : MonoBehaviour
{
    public void OnClickYesButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
        
        FindObjectOfType<SettingPanelController>().gameObject.SetActive(false);
        
<<<<<<< Updated upstream
        MainManager.Instance.Logout();
=======
        MainManager.Instance.CloseMainPanel();
        SceneManager.LoadScene("Login");
>>>>>>> Stashed changes
    }

    public void OnClickNoButton()
    {
          this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

}
