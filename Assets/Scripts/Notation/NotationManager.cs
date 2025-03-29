using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CardUI;

public class NotationManager : MonoBehaviour
{ 
    private List<(int,int)> values = new List<(int, int)>();
    
    public List<(int, int)> GetValues()
    {
        return values;
    }

    public void PrintAll() {
        Debug.Log(string.Join(", ", values));
    }
    public void AddClickIndex(int x, int y)
    {
        values.Add((x, y));
    }

    public void AddFinishTurn() { 
        values.Add((64, 64));
    }
    public void AddHowsFirst(Piece.Owner owner) {
        switch (owner) { 
            case Piece.Owner.PLAYER_A:
                values.Add((65, 64));
                break;
            case Piece.Owner.PLAYER_B:
                values.Add((64, 65));
                break;
        }
    }

    public void AddPieceType(Piece.PieceType pieceType) {
        switch (pieceType)
        {
            case Piece.PieceType.WARRIOR:
                values.Add((66,64));
                break;
            case Piece.PieceType.MAGICIAN:
                values.Add((66, 66));
                break;
            case Piece.PieceType.ARCHER:
                values.Add((66, 65));
                break;
            case Piece.PieceType.HEALER:
                values.Add((66, 68));
                break;
            case Piece.PieceType.RANCER:
                values.Add((66, 67));
                break;
            default:
                break;
        }
    }
}
