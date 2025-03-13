using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BuyingPanelController : MonoBehaviour
{
    public void OnClickitem1Button()
    {
        // 아이템 구매
    }

    public void OnClickitem2Button()
    {
        // 아이템 구매
    }

    public void OnClickitem3Button()
    {
        // 아이템 구매
    }

    public void OnClickitem4Button()
    {
        // 아이템 구매
    }

   public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
