using DG.Tweening;
using UnityEngine;

public class ErrorPanelController : MonoBehaviour
{
    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
