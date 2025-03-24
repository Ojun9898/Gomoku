using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BuyingPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text totalCoinText;
    private string[] BuyInfo;

    void Start()
    {
        SettingBuyInfo();
        SettingTotalCoin();
    }

    public void SettingBuyInfo()
    {
        BuyInfo = BuyManager.Instance.SetData();
    }

    public void SettingTotalCoin()
    {
        totalCoinText.text = BuyInfo[1];
    }

    public void OnClickitem1Button()
    {
        // 아이템 구매 (50코인 충전)
        BuyManager.Instance.UpdateTotalCoin(50);
        SettingBuyInfo();
        SettingTotalCoin();
    }

    public void OnClickitem2Button()
    {
        // 아이템 구매 (50코인 사용)
        BuyManager.Instance.UpdateTotalCoin(-50);
        SettingBuyInfo();
        SettingTotalCoin();
    }

    public void OnClickitem3Button()
    {
        // 아이템 구매
    }

    public void OnClickitem4Button()
    {
        // 아이템 구매
    }

    public void OnClickitem5Button()
    {
        // 아이템 구매
    }

    public void OnClickitem6Button()
    {
        // 아이템 구매
    }

   public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
