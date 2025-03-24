using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private Image cardImage;
    [SerializeField] private Button cardButton;

    // 흰색(PLAYER_A) 카드용 Sprite
    [SerializeField] private Sprite warriorWhite;
    [SerializeField] private Sprite mageWhite;
    [SerializeField] private Sprite archerWhite;
    [SerializeField] private Sprite lancerWhite;

    // 검정색(PLAYER_B) 카드용 Sprite
    [SerializeField] private Sprite warriorBlack;
    [SerializeField] private Sprite mageBlack;
    [SerializeField] private Sprite archerBlack;
    [SerializeField] private Sprite lancerBlack;

    private DeckManager.Card _cardData;
    private Piece.Owner _owner; // 카드 소유자 (PLAYER_A, PLAYER_B)

    // SetCard 오버로딩: 플레이어의 owner 정보를 함께 받음
    public void SetCard(DeckManager.Card card, Piece.Owner owner)
    {
        _cardData = card;
        _owner = owner;
        SetSpriteByType();
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(OnCardSelected);
    }

    // 기존 SetCard도 남겨두되, 기본 owner를 PLAYER_A로 처리 (원래 코드 호환용)
    public void SetCard(DeckManager.Card card)
    {
        SetCard(card, Piece.Owner.PLAYER_A);
    }

    // PieceType과 Owner에 따라 Sprite 설정
    private void SetSpriteByType()
    {
        switch(_cardData.pieceType)
        {
            case Piece.PieceType.WARRIOR:
                cardImage.sprite = (_owner == Piece.Owner.PLAYER_A) ? warriorWhite : warriorBlack;
                break;
            case Piece.PieceType.MAGE:
                cardImage.sprite = (_owner == Piece.Owner.PLAYER_A) ? mageWhite : mageBlack;
                break;
            case Piece.PieceType.ARCHER:
                cardImage.sprite = (_owner == Piece.Owner.PLAYER_A) ? archerWhite : archerBlack;
                break;
            case Piece.PieceType.RANCER:
                cardImage.sprite = (_owner == Piece.Owner.PLAYER_A) ? lancerWhite : lancerBlack;
                break;
            default:
                cardImage.sprite = null;
                break;
        }
    }

    private void OnCardSelected()
    {
        // HandManager로 카드 선택 이벤트를 전달
        HandManager hm = FindObjectOfType<HandManager>();
        if(hm != null)
        {
            hm.OnCardSelected(_cardData);
        }
        else
        {
            Debug.LogWarning("HandManager를 찾을 수 없습니다!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.transform.localScale = new Vector3(1.2f, 1.2f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.transform.localScale = new Vector3(1f, 1f, 1);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardSelected();
    }

    // 추가: 카드 데이터를 반환하는 메소드 (필요시)
    public DeckManager.Card GetCardData()
    {
        return _cardData;
    }
}
