using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SelectPanelController : MonoBehaviour
{

    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    public void OnClickSelecButton()
    {
        
    }

    public void OnClickResetButton()
    {

    }
}
