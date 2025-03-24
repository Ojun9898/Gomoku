using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<Card> deck = new List<Card>();
    [SerializeField] private List<Card> playerACards = new List<Card>();
    [SerializeField] private List<Card> playerBCards = new List<Card>();

    /// <summary>
    /// 카드 데이터 구조체
    /// </summary>
    [System.Serializable]
    public class Card
    {
        public Piece.PieceType pieceType;
        public int pieceCost;
        public Sprite cardSprite; // 카드 이미지
    }

    /// <summary>
    /// 카드 덱 초기화
    /// </summary>
    public void InitializeDeck()
    {
        // 덱에 카드 추가 (예시)
        for (int i = 0; i < 15; i++)
        {
            int typeIndex = Random.Range(0, 4);

            switch (typeIndex)
            {
                case 0:
                    deck.Add(new Card { pieceType = Piece.PieceType.WARRIOR, pieceCost = 1, cardSprite = null });
                    break;
                
                case 1:
                    deck.Add(new Card { pieceType = Piece.PieceType.MAGE, pieceCost = 2, cardSprite = null });
                    break;
                
                case 2:
                    deck.Add(new Card { pieceType = Piece.PieceType.ARCHER, pieceCost = 3, cardSprite = null });
                    break;
                
                case 3:
                    deck.Add(new Card { pieceType = Piece.PieceType.RANCER, pieceCost = 4, cardSprite = null });
                    break;
            }
        } 
        
        DealCards();
    }

    /// <summary>
    /// 덱을 섞는 메소드
    /// </summary>
    private void ShuffleDeck(List<Card> decks)
    {
        for (int i = 0; i < decks.Count; i++)
        {
            Card temp = decks[i];
            int randomIndex = UnityEngine.Random.Range(i, decks.Count);
            decks[i] = decks[randomIndex];
            decks[randomIndex] = temp;
        }
    }

    /// <summary>
    /// 카드를 플레이어에게 분배
    /// </summary>
    private void DealCards()
    {
        playerACards.Clear();
        playerBCards.Clear();
        
        
        
        // 플레이어 A와 B에게 각각 15장씩 카드 분배
        for (int i = 0; i < 15; i++)
        {
            playerACards.Add(deck[i]);
            playerBCards.Add(deck[i]);
        }
        
        ShuffleDeck(playerACards);
        ShuffleDeck(playerBCards);
    }

    /// <summary>
    /// 카드 선택 시 해당 카드로 유닛 생성
    /// </summary>
    /// <param name="pieceType"></param>
    /// <param name="position"></param>
    public void PlayCard(Piece.PieceType pieceType, Vector3 position)
    {
        GameObject piecePrefab = null;

        switch (pieceType)
        {
            // TODO: 플레이어 타입에 따라서 path값 수정.
            case Piece.PieceType.WARRIOR:
                piecePrefab = Resources.Load<GameObject>("Units/Warrior"); // Prefab 경로 예시
                break;
            case Piece.PieceType.MAGE:
                piecePrefab = Resources.Load<GameObject>("Units/Magician");
                break;
            case Piece.PieceType.ARCHER:
                piecePrefab = Resources.Load<GameObject>("Units/Archer");
                break;
            case Piece.PieceType.RANCER:
                piecePrefab = Resources.Load<GameObject>("Units/Rancer");
                break;
            // 추가적인 PieceType에 대해 Prefab 로딩
        }

        if (piecePrefab != null)
        {
            Instantiate(piecePrefab, position, Quaternion.identity);
        }
    }
}
