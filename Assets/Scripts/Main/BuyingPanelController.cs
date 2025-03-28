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
<<<<<<< Updated upstream
=======
    private int itemPrice = 50;
>>>>>>> Stashed changes

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
<<<<<<< Updated upstream
        BuyManager.Instance.UpdateTotalCoin(50);
        SettingBuyInfo();
        SettingTotalCoin();
=======
        itemPrice = 50;
        Buying(itemPrice);

>>>>>>> Stashed changes
    }

    public void OnClickitem2Button()
    {
        // 아이템 구매 (50코인 사용)
<<<<<<< Updated upstream
        BuyManager.Instance.UpdateTotalCoin(-50);
        SettingBuyInfo();
        SettingTotalCoin();
=======
        itemPrice = -50;
        Buying(itemPrice);
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
   public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
=======
    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    public bool CheckHaveCoin(int coins, int itemPrice)
    {
        // itemPrice가 음수일 때만 검사
        if (itemPrice < 0 && coins < -itemPrice)
        {
            MainManager.Instance.ShowErrorPanel("코인이 부족합니다.");
            return false;
        }

        return true;
    }


    public void Buying(int itemPrice)
    {
        int haveCoins = BuyManager.Instance.GetCoins();

        if (CheckHaveCoin(haveCoins, itemPrice))
        {
            BuyManager.Instance.UpdateTotalCoin(itemPrice);
            SettingBuyInfo();
            SettingTotalCoin();
        }
>>>>>>> Stashed changes
    }
}
