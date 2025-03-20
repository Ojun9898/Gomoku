using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Piece : HaveHp
{
    public enum PieceType
    {
        WARRIOR,
        RANCER,
        MAGE,
        ARCHER
    }
    
    public enum AttackType {
        NONE,
        CHOOSE_ATTACK,
        RANGE_ATTACK,
        BUFF
    }
    
    public PieceType pieceType;
    public AttackType attackType;
    public int[] RangeAttackRange;
    [SerializeField] private int pieceCost;
    [SerializeField] private int attackPower = 1;
    [SerializeField] private int attackRange = 1;

    public enum Owner
    {
        NONE,
        PLAYER_A,
        PLAYER_B
    }
    public Owner pieceOwner;

    public Owner GetPieceOwner()
    {
        return pieceOwner;
    }

    public void SetPieceOwner(Owner pieceOwner)
    {
        this.pieceOwner = pieceOwner;
    }


    public int GetAttackRange()
    {
        return attackRange;
    }
    public void SetAttackRange(int attackRange)
    {
        this.attackRange = attackRange;
    }


    public int GetAttackPower()
    {
        return attackPower;
    }
    public void SetAttackPower(int attackPower)
    {
        this.attackPower = attackPower;
    }

    public int GetPieceCost()
    {
        return pieceCost;
    }

    public void SetPieceCost(int pieceCost)
    {
        this.pieceCost = pieceCost;
    }






    public void ChoseAttack(Piece pc,int attackPower) { 
        pc.Hp -= attackPower;
    }
    public void Buff(Piece pc, int attackPower) { 
        pc.Hp += attackPower;
    }
    public void ChoseAttack(Obstacle oc, int attackPower)
    {
        oc.Hp -= attackPower;
    }
}
