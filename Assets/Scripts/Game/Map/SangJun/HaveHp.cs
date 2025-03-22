using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaveHp : MonoBehaviour
{
    private int _hp= 5;
    public int Hp
    {
        get => _hp;
        set
        {
            _hp = value;
            if (_hp <= 0)
            {
                GameManager.Instance.Mc.tiles[GameManager.Instance.currentClickedTileindex].JustBeforDestroyPiece?.Invoke();
                GameManager.Instance.Mc.tiles[GameManager.Instance.currentClickedTileindex].JustBeforDestroyObstacle?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
