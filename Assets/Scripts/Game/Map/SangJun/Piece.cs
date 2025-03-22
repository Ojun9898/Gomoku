using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
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
    
    public AttackType attackType;
    public bool isAlreadyAttack;
    public int[] rangeAttackRange;
    public int cost;
    
    [SerializeField] private int attackPower=1;
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
        return cost;
    }

    public void SetPieceCost(int pieceCost)
    {
        this.cost = pieceCost;
    }






    public void ChoseAttack(Piece piece,int attackPower) { 
        piece.Hp -= attackPower;
    }
    public void Buff(Piece piece, int attackPower) { 
        piece.Hp += attackPower;
    }
    public void ChoseAttack(Obstacle oc, int attackPower)
    {
        oc.Hp -= attackPower;
    }
}
