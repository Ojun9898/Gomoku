using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdditionalRangeBuff : Buff
{
    public override void SetBuffContent()
    {
        base.BuffContent = (Piece pc) =>
        {
            if (pc != null)
            {
                var Range = pc.GetAttackRange();
                pc.SetAttackRange(Range + 1);
            }
            // 지금은 1회성 즉 다른  색깔이 들어오면 사라지니
            // 버프가 적용된 땅임을 알리는 다른 방법을 생각해내야함 Todo : 예를 들면 오브젝트 추가 배치..
            GameManager.Instance.Mc.tiles[GameManager.Instance.currentClickedTileIndex].GetComponent<SpriteRenderer>().color = Color.black;
            MessageManager.Instance.ShowMessagePanel($"말의 사거리가 1 증가 하였습니다!");
        };
    }
}
