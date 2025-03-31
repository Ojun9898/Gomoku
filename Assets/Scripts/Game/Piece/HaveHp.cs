using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using static Piece;

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
                StartCoroutine(DestroyAndWait(gameObject));
            }
        }
    }

    IEnumerator DestroyAndWait(GameObject obj)
    {
        GameManager.Instance.Mc.tiles[GameManager.Instance.CurrentClickedTileIndex]?.JustBeforeDestroyPieceOrObstacle?.Invoke();
        yield return null; // 1 프레임 대기
        Destroy(obj); // 현재 프레임이 끝난 후 삭제됨
        yield return null; // 1 프레임 대기
    }
}
