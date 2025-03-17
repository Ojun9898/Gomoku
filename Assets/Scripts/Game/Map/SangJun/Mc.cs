using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class Mc : MonoBehaviour
{
    public List<Tile> tiles;
    int width = 4;

    Buff[] buffs = new Buff[4];
    void Start()
    {
        for (int i = 0; i < width; i++)
        {
            tiles[i].SetBuff(new AdditionalAttackPowerBuff());
        }
        for (int i = width; i < width+width; i++)
        {
            tiles[i].SetBuff(new AdditionalRangeBuff());
        }    
        for (int i = width + width; i < width+width+width; i++)
        {
            tiles[i].SetBuff(new AdditionalHpBuff());
        }
    }
}
