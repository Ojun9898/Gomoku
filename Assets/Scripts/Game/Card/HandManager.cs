using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HandManager : MonoBehaviour
{
    [SerializeField] private GameObject cardUIPrefab;    // CardUI 프리팹 (CardUI 스크립트가 붙어 있음)
    public GameObject playerAHandPanel;      // 카드 UI가 표시될 부모 오브젝트 (예: Canvas 하위 패널)
    public GameObject playerBHandPanel;      // 카드 UI가 표시될 부모 오브젝트 (예: Canvas 하위 패널)
    
    [SerializeField] private int handSize = 5; // 초기 손패 개수
    public List<DeckManager.Card> playerAHandCards = new List<DeckManager.Card>();
    public List<DeckManager.Card> playerBHandCards = new List<DeckManager.Card>();

    // 플레이어의 Owner 타입을 지정 (인스펙터에서 바인딩 가능)
    public Piece.Owner playerOwner;

    private DeckManager _deckManager;
    private Tile _selectedTile;
    public bool isAlreadySetPiece;

    // 플레이어 덱에서 handSize만큼 카드를 Pop하여 손패에 추가 후 UI 생성
    public void InitializeHand(DeckManager deckManager)
    {
        _deckManager = deckManager;
        
        for (int i = 0; i < handSize; i++)
        {
            // Player A의 덱에서 카드 Pop
            DeckManager.Card card = _deckManager.PopCard(GetPlayerDeck(_deckManager.playerACards));
            if (card != null)
            {
                playerAHandCards.Add(card);
                CreateCardUI(card, Piece.Owner.PLAYER_A);
            }

            // Player B의 덱에서 카드 Pop
            card = _deckManager.PopCard(GetPlayerDeck(_deckManager.playerBCards));
            if (card != null)
            {
                playerBHandCards.Add(card);
                CreateCardUI(card, Piece.Owner.PLAYER_B);
            }
        }
    }

    // 매개변수로 deck을 받아와 원하는 덱을 가져온다.
    public List<DeckManager.Card> GetPlayerDeck(List<DeckManager.Card> deck)
    {
        return deck;
    }

    // 카드 UI를 동적으로 생성하여 handPanel에 추가
    // 카드 UI를 동적으로 생성하여 해당 플레이어의 Hand Panel에 추가하는 메소드
    public void CreateCardUI(DeckManager.Card card, Piece.Owner owner)
    {
        GameObject parentPanel = (owner == Piece.Owner.PLAYER_A) ? playerAHandPanel : playerBHandPanel;
        GameObject cardUIObj = Instantiate(cardUIPrefab, parentPanel.transform);
        CardUI cardUIScript = cardUIObj.GetComponent<CardUI>();
        // 플레이어의 Owner 타입을 함께 전달하여 올바른 Sprite가 할당되도록 합니다.
        cardUIScript.SetCard(card, owner);
    }

    // 카드가 선택되었을 때 호출됨 (카드 클릭 → 타일 선택 후 말 생성)
    public void OnCardSelected(DeckManager.Card selectedCard)
    {
        if (_selectedTile == null)
        {
            Debug.Log("먼저 타일을 선택하세요.");
            return;
        }

        if (isAlreadySetPiece == false)
        {
            // GameManager의 SetPieceAtTile 메소드를 사용하여 선택된 타일에 말을 생성합니다.
            _deckManager.PlayCard(selectedCard.pieceType, playerOwner);
            GameObject pieceInstance = GameManager.Instance.SetPieceAtTile(_selectedTile.tileNumber);
            isAlreadySetPiece = true;
            Piece pieceComponent = pieceInstance.GetComponent<Piece>();
            _selectedTile.Piece = pieceComponent;
            if (pieceComponent != null)
            {
                // 카드에 따른 말의 속성(예: 소유자 설정)을 지정합니다.
                pieceComponent.SetPieceOwner(playerOwner);
                // (필요 시 카드 데이터(selectedCard) 기반 추가 설정 가능)

                // 타일에 버프가 있을 경우 적용합니다.
                if (_selectedTile.GetBuff() != null)
                {
                    _selectedTile.GetBuff().On(pieceComponent);
                }
            }

            // 카드 선택 시 선택한 이미지, 카드 패널 비활성화
            if (playerOwner == Piece.Owner.PLAYER_A)
            {
                playerAHandPanel.SetActive(false);
                _selectedTile.clickedImageObj.SetActive(false);
            }

            if (playerOwner == Piece.Owner.PLAYER_B)
            {
                playerBHandPanel.SetActive(false);
                _selectedTile.clickedImageObj.SetActive(false);
            }

            // 손패에서 해당 카드를 제거하고 UI를 갱신합니다.
            if (playerOwner == Piece.Owner.PLAYER_A)
            {
                playerAHandCards.Remove(selectedCard);
            }
            else if (playerOwner == Piece.Owner.PLAYER_B)
            {
                playerBHandCards.Remove(selectedCard);
            }

            RefreshHandUI(playerOwner);

            // 선택된 타일 초기화
            _selectedTile.ResetClickCount();
            _selectedTile = null;
        }
        else { 
            MessageManager.Instance.ShowMessagePanel("이미 말을 두었습니다.");
        }
    }


    // 타일 클릭 시 HandManager에서 호출: 선택된 타일 저장
    public void SetSelectedTile(Tile tile)
    {
        _selectedTile = tile;
        if(_selectedTile.isForbiddenMove)
        {
            MessageManager.Instance.ShowMessagePanel("금수 입니다.");
            _selectedTile = null;
            return;
        }
        else if (_selectedTile.GetObstacle() != null)
        {
            MessageManager.Instance.ShowMessagePanel("장애물이 있습니다.");
            _selectedTile = null;
            return;
        }
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

            foreach (var card in playerAHandCards)
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

            foreach (var card in playerBHandCards)
            {
                CreateCardUI(card, playerOwner);
            }
        }
    }

    public List<DeckManager.Card> GetPlayerAorBHandCards(Piece.Owner owner) {
        if (owner == Piece.Owner.PLAYER_B) { 
            return playerBHandCards;
        }else if(owner == Piece.Owner.PLAYER_A)
        {
            return playerAHandCards;
        }
        return null;
    }
 
}
