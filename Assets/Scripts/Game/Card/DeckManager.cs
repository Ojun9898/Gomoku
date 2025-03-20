using System.Collections.Generic;
using UnityEngine;

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
        deck.Add(new Card { pieceType = Piece.PieceType.WARRIOR, pieceCost = 1, cardSprite = null }); // 실제 Sprite는 에디터에서 할당
        deck.Add(new Card { pieceType = Piece.PieceType.MAGE, pieceCost = 2, cardSprite = null });
        deck.Add(new Card { pieceType = Piece.PieceType.ARCHER, pieceCost = 3, cardSprite = null });
        deck.Add(new Card { pieceType = Piece.PieceType.RANCER, pieceCost = 4, cardSprite = null });
        // ... 추가로 덱에 카드들을 넣어주세요.

        ShuffleDeck();
        DealCards();
    }

    /// <summary>
    /// 덱을 섞는 메소드
    /// </summary>
    private void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            Card temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
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
            playerBCards.Add(deck[15 + i]);
        }
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
            case Piece.PieceType.WARRIOR:
                piecePrefab = Resources.Load<GameObject>("Prefabs/Warrior"); // Prefab 경로 예시
                break;
            case Piece.PieceType.MAGE:
                piecePrefab = Resources.Load<GameObject>("Prefabs/Mage");
                break;
            case Piece.PieceType.ARCHER:
                piecePrefab = Resources.Load<GameObject>("Prefabs/Archer");
                break;
            // 추가적인 PieceType에 대해 Prefab 로딩
        }

        if (piecePrefab != null)
        {
            Instantiate(piecePrefab, position, Quaternion.identity);
        }
    }
}
