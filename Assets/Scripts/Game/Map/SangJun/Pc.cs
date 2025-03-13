using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pc : MonoBehaviour
{
    public enum Owner
    {
        NONE,
        PLAYER_A,
        PLAYER_B
    }
    private Owner _pieceOwner;

    public Owner GetPieceOwner()
    {
        return _pieceOwner;
    }

    public void SetPieceOwner(Owner pieceOwner)
    {
        _pieceOwner = pieceOwner;
    }

    public void Attack() { 
    
    }
}
