using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class HaveHp : MonoBehaviour
{
    public int hp= 5;
    public int Hp
    {
        get => hp;
        set
        {
            hp = value;
            if (hp <= 0)
            {
                GameManager.Instance.Mc.tiles[GameManager.Instance.CurrentClickedTileIndex].JustBeforeDestroyPiece?.Invoke();
                GameManager.Instance.Mc.tiles[GameManager.Instance.CurrentClickedTileIndex].JustBeforeDestroyObstacle?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
