using System.Collections.Generic;
using UnityEngine;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardUIPrefab;    // CardUI 프리팹 (CardUI 스크립트가 붙어 있음)
    public GameObject playerAHandPanel;      // 카드 UI가 표시될 부모 오브젝트 (예: Canvas 하위 패널)
    public GameObject playerBHandPanel;      // 카드 UI가 표시될 부모 오브젝트 (예: Canvas 하위 패널)
    
    [SerializeField] private int handSize = 5; // 초기 손패 개수
    private List<DeckManager.Card> _playerAHandCards = new List<DeckManager.Card>();
    private List<DeckManager.Card> _playerBHandCards = new List<DeckManager.Card>();

    // 플레이어의 Owner 타입을 지정 (인스펙터에서 바인딩 가능)
    public Piece.Owner playerOwner;

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
            // Player A의 덱에서 카드 Pop
            DeckManager.Card card = _deckManager.PopCard(GetPlayerDeck(_deckManager.playerACards));
            if (card != null)
            {
                _playerAHandCards.Add(card);
                CreateCardUI(card, Piece.Owner.PLAYER_A);
            }

            // Player B의 덱에서 카드 Pop
            card = _deckManager.PopCard(GetPlayerDeck(_deckManager.playerBCards));
            if (card != null)
            {
                _playerBHandCards.Add(card);
                CreateCardUI(card, Piece.Owner.PLAYER_B);
            }
        }
    }

    // 매개변수로 deck을 받아와 원하는 덱을 가져온다.
    private List<DeckManager.Card> GetPlayerDeck(List<DeckManager.Card> deck)
    {
        return deck;
    }

    // 카드 UI를 동적으로 생성하여 handPanel에 추가
// 카드 UI를 동적으로 생성하여 해당 플레이어의 Hand Panel에 추가하는 메소드
    private void CreateCardUI(DeckManager.Card card, Piece.Owner owner)
    {
        GameObject parentPanel = (owner == Piece.Owner.PLAYER_A) ? playerAHandPanel : playerBHandPanel;
        GameObject cardUIObj = Instantiate(cardUIPrefab, parentPanel.transform);
        CardUI cardUIScript = cardUIObj.GetComponent<CardUI>();
        // 플레이어의 Owner 타입을 함께 전달하여 올바른 Sprite가 할당되도록 합니다.
        cardUIScript.SetCard(card, owner);
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
        if (playerOwner == Piece.Owner.PLAYER_A)
        {
            _playerAHandCards.Remove(selectedCard);
        }
        else if (playerOwner == Piece.Owner.PLAYER_B)
        {
            _playerBHandCards.Remove(selectedCard);
        }
        RefreshHandUI(playerOwner);

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
    private void RefreshHandUI(Piece.Owner owner)
    {
        if (owner == Piece.Owner.PLAYER_A)
        {
            foreach (Transform child in playerAHandPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var card in _playerAHandCards)
            {
                CreateCardUI(card, playerOwner);
            }
        }
        else if (owner == Piece.Owner.PLAYER_B)
        {
            foreach (Transform child in playerBHandPanel.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var card in _playerBHandCards)
            {
                CreateCardUI(card, playerOwner);
            }
        }
    }
}
