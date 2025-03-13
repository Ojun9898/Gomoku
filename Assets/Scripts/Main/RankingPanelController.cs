using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RankingPanelController : MonoBehaviour
{
    // 플레이어 랭킹, 점수 불러오기
    // 랭킹 데이터 불러오기
    // 판넬에 랭킹 데이터 표시

    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-300f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
