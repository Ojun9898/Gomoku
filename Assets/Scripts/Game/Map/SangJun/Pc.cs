using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class Pc : HaveHp
{
 
    public enum AttackType {
        NONE,
        CHOOSE_ATTACK,
        RANGE_ATTACK,
        BUFF
    }

    public AttackType _attackType;
    public int[] RangeAttackRange;
    [SerializeField] private int _attackPower=1;
    [SerializeField] private int _attackRange = 1;

    public enum Owner
    {
        NONE,
        PLAYER_A,
        PLAYER_B
    }
    public Owner _pieceOwner;

    public Owner GetPieceOwner()
    {
        return _pieceOwner;
    }

    public void SetPieceOwner(Owner pieceOwner)
    {
        _pieceOwner = pieceOwner;
    }


    public int GetAttackRange()
    {
        return _attackRange;
    }
    public void SetAttackRange(int attackRange)
    {
        _attackRange = attackRange;
    }


    public int GetAttackPower()
    {
        return _attackPower;
    }
    public void SetAttackPower(int attackPower)
    {
        _attackPower = attackPower;
    }






    public void ChoseAttack(Pc pc,int attackPower) { 
        pc.Hp -= attackPower;
    }
    public void Buff(Pc pc, int attackPower) { 
        pc.Hp += attackPower;
    }
    public void ChoseAttack(Obstacle oc, int attackPower)
    {
        oc.Hp -= attackPower;
    }


}
