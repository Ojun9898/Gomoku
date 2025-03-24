using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private Transform handPanel;      // 카드 UI가 표시될 부모 오브젝트 (예: Canvas 하위 패널)
    [SerializeField] private GameObject cardUIPrefab;    // CardUI 프리팹 (CardUI 스크립트가 붙어 있음)
    [SerializeField] private int handSize = 5;           // 초기 손패 개수
    private List<DeckManager.Card> _handCards = new List<DeckManager.Card>();

    // 플레이어의 Owner 타입을 지정 (인스펙터에서 바인딩 가능)
    public Piece.Owner playerOwner = Piece.Owner.PLAYER_A;

    private DeckManager _deckManager;
    private Tile _selectedTile;

    private void Awake()
    {
        _deckManager = FindObjectOfType<DeckManager>();
    }

    private void Start()
    {
        // 게임 시작 시 플레이어의 손패 초기화 (예: handSize만큼 Pop)
        InitializeHand();
    }

    // 플레이어 덱에서 handSize만큼 카드를 Pop하여 손패에 추가 후 UI 생성
    private void InitializeHand()
    {
        for (int i = 0; i < handSize; i++)
        {
            DeckManager.Card card = _deckManager.PopCard(GetPlayerDeck());
            if (card != null)
            {
                _handCards.Add(card);
                CreateCardUI(card);
            }
        }
    }

    // 예시: 플레이어 A의 덱을 사용 (원하는 덱으로 수정 가능)
    private List<DeckManager.Card> GetPlayerDeck()
    {
        // playerACards는 private이지만, 필요한 경우 public 접근자를 추가하거나 여기서 처리합니다.
        // 이 예제에서는 단순히 deckManager 내부의 playerACards를 반환한다고 가정합니다.
        return _deckManager.playerACards;
    }

    // 카드 UI를 동적으로 생성하여 handPanel에 추가
    private void CreateCardUI(DeckManager.Card card)
    {
        GameObject cardUIObj = Instantiate(cardUIPrefab, handPanel);
        CardUI cardUIScript = cardUIObj.GetComponent<CardUI>();
        // 플레이어의 Owner 타입을 함께 전달하여 올바른 Sprite가 할당되도록 합니다.
        cardUIScript.SetCard(card, playerOwner);
    }

    // HandManager가 CardUI에서 호출하는 메소드: 카드가 선택되었을 때 호출됨
    public void OnCardSelected(DeckManager.Card selectedCard)
    {
        if (_selectedTile == null)
        {
            Debug.Log("먼저 타일을 선택하세요.");
            return;
        }

        // 선택된 카드에 따른 유닛 생성: 선택된 타일의 위치에 생성
        _deckManager.PlayCard(selectedCard.pieceType, _selectedTile.transform.position, playerOwner);

        // 손패에서 해당 카드를 제거하고 UI 갱신
        _handCards.Remove(selectedCard);
        RefreshHandUI();

        // 선택한 타일 초기화 (원한다면)
        _selectedTile = null;
    }

    // 타일 클릭 시 HandManager에서 호출: 선택된 타일 저장
    public void SetSelectedTile(Tile tile)
    {
        _selectedTile = tile;
        Debug.Log("타일 선택됨: " + tile.tileNumber);
    }

    // 현재 손패 UI를 갱신하는 메소드 (간단하게 모든 자식을 제거하고 다시 생성)
    private void RefreshHandUI()
    {
        foreach (Transform child in handPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (var card in _handCards)
        {
            CreateCardUI(card);
        }
    }
}
