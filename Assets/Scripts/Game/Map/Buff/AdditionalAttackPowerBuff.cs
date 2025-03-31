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
            MessageManager.Instance.ShowMessagePanel($"말의 공격력이 1 증가 하였습니다!");
        };
    }
}
