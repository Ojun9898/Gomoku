using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<Card> deck = new List<Card>();
    public List<Card> playerACards = new List<Card>();
    public List<Card> playerBCards = new List<Card>();
    GameObject WarriorWhite;
    GameObject WarriorBlack;
    GameObject ArcherWhite ;
    GameObject ArcherBlack ;
    GameObject MagicianWhite ;
    GameObject MagicianBlack ;
    GameObject HealerWhite ;
    GameObject HealerBlack ;
    GameObject RancerWhite ;
    GameObject RancerBlack ;

    /// <summary>
    /// 카드 데이터 구조체
    /// </summary>
    [Serializable]
    public class Card
    {
        public Piece.PieceType pieceType;
        public int pieceCost;
        public Sprite cardSprite; // 카드 이미지
        // 추가: 카드가 어느 플레이어의 손패인지 나타내기 위해 owner를 추가 (기본값 PLAYER_A)
        public Piece.Owner owner = Piece.Owner.PLAYER_A;
    }

    /// <summary>
    /// 카드 덱 초기화
    /// </summary>
    public void InitializeDeck()
    {
         WarriorWhite = Resources.Load<GameObject>("Units/WarriorWhite");
        WarriorBlack = Resources.Load<GameObject>("Units/WarriorBlack");
        ArcherWhite = Resources.Load<GameObject>("Units/ArcherWhite");
        ArcherBlack = Resources.Load<GameObject>("Units/ArcherBlack");
        MagicianWhite = Resources.Load<GameObject>("Units/MagicianWhite");
        MagicianBlack = Resources.Load<GameObject>("Units/MagicianBlack");
        HealerWhite = Resources.Load<GameObject>("Units/HealerWhite");
        HealerBlack = Resources.Load<GameObject>("Units/HealerBlack");
        RancerWhite = Resources.Load<GameObject>("Units/RancerWhite");
        RancerBlack = Resources.Load<GameObject>("Units/RancerBlack");

        // 덱에 카드 추가 (예시)
        for (int i = 0; i < 15; i++)
        {
            int typeIndex = Random.Range(0, 5);

            switch (typeIndex)
            {
                case 0:
                    deck.Add(new Card { pieceType = Piece.PieceType.WARRIOR, pieceCost = 1, cardSprite = null });
                    break;
                
                case 1:
                    deck.Add(new Card { pieceType = Piece.PieceType.ARCHER, pieceCost = 2, cardSprite = null });
                    break;
                
                case 2:
                    deck.Add(new Card { pieceType = Piece.PieceType.MAGICIAN, pieceCost = 3, cardSprite = null });
                    break;
                
                case 3:
                    deck.Add(new Card { pieceType = Piece.PieceType.HEALER, pieceCost = 3, cardSprite = null });
                    break;
                
                case 4:
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
            int randomIndex = Random.Range(i, decks.Count);
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
            deck[i].owner = Piece.Owner.PLAYER_A;
            playerACards.Add(deck[i]);
        }
        
        for (int i = 0; i < 15; i++)
        {
            deck[i].owner = Piece.Owner.PLAYER_B;
            playerBCards.Add(deck[i]);
        }
        
        ShuffleDeck(playerACards);
        ShuffleDeck(playerBCards);
    }

    /// <summary>
    /// 카드 선택 시 해당 카드로 유닛 생성
    /// </summary>
    /// <param name="pieceType"></param>
    public void PlayCard(Piece.PieceType pieceType, Piece.Owner owner)
    {
        GameObject piecePrefab = null;

        switch (pieceType)
        {
           


            case Piece.PieceType.WARRIOR:
                piecePrefab = owner == Piece.Owner.PLAYER_A ? WarriorWhite : WarriorBlack;
                piecePrefab.GetComponent<Piece>().hp = 3;
                break;
            case Piece.PieceType.ARCHER:
                piecePrefab = owner == Piece.Owner.PLAYER_A ? ArcherWhite : ArcherBlack;
                piecePrefab.GetComponent<Piece>().hp = 3;
                break;
            case Piece.PieceType.MAGICIAN:
                piecePrefab = owner == Piece.Owner.PLAYER_A ? MagicianWhite : MagicianBlack;
                piecePrefab.GetComponent<Piece>().hp = 2;
                break;
            case Piece.PieceType.HEALER:
                piecePrefab = owner == Piece.Owner.PLAYER_A ? HealerWhite : HealerBlack;
                piecePrefab.GetComponent<Piece>().hp = 4;
                break;
            case Piece.PieceType.RANCER:
                piecePrefab = owner == Piece.Owner.PLAYER_A ? RancerWhite : RancerBlack;
                piecePrefab.GetComponent<Piece>().hp = 5;
                break;
            // 추가 PieceType 처리 가능
        }

        if (piecePrefab != null)
        {
            GameManager.Instance.piece = piecePrefab;
        }
    }

    
    // 추가: 덱에서 카드를 Pop 방식으로 받아오는 메소드 (삭제 없이 추가)
    public Card PopCard(List<Card> deckList)
    {
        if (deckList.Count > 0)
        {
            Card card = deckList[deckList.Count - 1];
            deckList.RemoveAt(deckList.Count - 1);
            return card;
        }
        Debug.LogWarning("덱이 비었습니다!");
        return null;
    }
}
