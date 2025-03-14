using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.Mathematics;
using UnityEngine;

public class Pc : MonoBehaviour
{
    public enum AttackType {
        NONE,
        CHOOSE_ATTACK,
        RANGE_ATTACK,
        BUFF
    }

    public AttackType _attackType;
    public int[] RangeAttackRange;
    [SerializeField]private int _attackRange = 1;

    private int _hp = 5;
    public int Hp {get{return _hp;} set{
                _hp = value;
                if(_hp <= 0){
                    Destroy(this.gameObject);
            }
            }}
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





    public void ChoseAttack(Pc pc) { 
        pc.Hp -= 1;
        Debug.Log("     ߽  ϴ ");
    }
    public void Buff(Pc pc) { 
        pc.Hp += 1;
        Debug.Log("   ߽  ϴ ");
    }
    
}
