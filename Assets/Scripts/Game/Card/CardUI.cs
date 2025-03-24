using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;
    private DeckManager.Card _cardData;

    /// <summary>
    /// 카드 정보를 설정하는 메소드
    /// </summary>
    /// <param name="card"></param>
    public void SetCard(DeckManager.Card card)
    {
        _cardData = card;
        cardImage.sprite = card.cardSprite;
        cardButton.onClick.AddListener(OnCardSelected);
    }

    /// <summary>
    /// 카드 선택 시 호출되는 메소드
    /// </summary>
    private void OnCardSelected()
    {
        // GameManager.Instance.SelectCard(_cardData.pieceType);
    }

    /// <summary>
    /// 마우스 올렸을 때 확대 애니메이션
    /// </summary>
    public void OnPointerEnter()
    {
        cardImage.transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }

    /// <summary>
    /// 마우스 벗어났을 때 원래 크기로 복원
    /// </summary>
    public void OnPointerExit()
    {
        cardImage.transform.localScale = new Vector3(1f, 1f, 1);
    }
}