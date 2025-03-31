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
                StartCoroutine(DestroyAndWait(gameObject));
            }
        }
    }

    IEnumerator DestroyAndWait(GameObject obj)
    {
        Destroy(obj); // 현재 프레임이 끝난 후 삭제됨
        yield return null; // 1 프레임 대기
        Debug.Log("객체가 삭제된 후 실행됨!");
    }
}
