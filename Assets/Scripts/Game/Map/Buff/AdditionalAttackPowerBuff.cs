using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalAttackPowerBuff : Buff
{
    public override void SetBuffContent()
    {
        base.BuffContent = (Piece pc) =>
        {
            if(pc != null)
            {
                var Power = pc.GetAttackPower();
                pc.SetAttackPower(Power+1) ;
            }
            GameManager.Instance.Mc.tiles[GameManager.Instance.currentClickedTileIndex].GetComponent<SpriteRenderer>().color = Color.cyan;
            Debug.Log(pc.name + "의 공격력 1 증가 " + pc.name+"의 현재 공격력 : "+pc.GetAttackPower());
        };
    }
}
