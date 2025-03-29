using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutPanelController : MonoBehaviour
{
    public void OnClickYesButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
        
        FindObjectOfType<SettingPanelController>().gameObject.SetActive(false);
        
        MainManager.Instance.CloseMainPanel();
        SceneManager.LoadScene("Login");
    }

    public void OnClickNoButton()
    {
          this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

}
