using DG.Tweening;
using TMPro;
using UnityEngine;

public class BuyingPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text totalCoinText;
    private string[] BuyInfo;
    private int itemPrice = 50;

    void Start()
    {
        SettingBuyInfo();
        SettingTotalCoin();
    }

    public void SettingBuyInfo()
    {
        BuyInfo = BuyManager.Instance.SetCSVFileData();
    }

    public void SettingTotalCoin()
    {
        totalCoinText.text = BuyInfo[1];
    }

    public void OnClickitem1Button()
    {
        itemPrice = 50;
        Buying(itemPrice, "Coin"); // Coin 충전은 Coin

    }

    public void OnClickitem2Button()
    {
        // 아이템 구매 (50코인 사용)
        itemPrice = -50;
        Buying(itemPrice, "timer"); // 아이템 이름: Timer
    }

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


    public void Buying(int itemPrice, string buyItem)
    {
        int haveCoins = BuyManager.Instance.GetCoins();

        if (CheckHaveCoin(haveCoins, itemPrice))
        {
            BuyManager.Instance.UpdateTotalCoin(itemPrice);
            SettingBuyInfo();
            SettingTotalCoin();

            if (buyItem != "Coin")
                BuyManager.Instance.UpdateBuyItems(buyItem);
        }
    }
}
