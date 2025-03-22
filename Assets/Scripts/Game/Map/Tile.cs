using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

  
    [SerializeField] private GameObject cursorImageObj;
    [SerializeField] private GameObject ClickedImageObj;
    private int _tileClickCount;
    private bool isNeedOneClick;
    public int tileNumber;

    [SerializeField] private Obstacle obstacle;
    public bool isForbiddenMove;
    private Buff _buff;
    public Pc _piece { get; private set; }

    public Action JustBeforDestroyPiece;
    public Action JustBeforDestroyObstacle;


    /// <summary>
    ///  타일의 obstacle,buff,_piece를 초기화 하는 메소드 입니다
    /// </summary>
    public void ResetAll() {
        obstacle = null;
        _buff = null;
        _piece = null;
    }

    /// <summary>
    /// 클릭한 상황을 초기화 하는 메소드 입니다
    /// </summary>
    public void ResetClick()
    {
        ClickedImageObj.SetActive(false);
        _tileClickCount = 0;
    }

    public Obstacle GetObstacle() { 
            return obstacle;
    }

    public void SetObstacle(Obstacle obstacle)
    {
        this.obstacle = obstacle;
    }
    
    public Buff GetBuff()
    {
        return _buff;
    } 
    public void SetBuff(Buff buff)
    {
        this._buff = buff;
    }


    /// <summary>
    /// 타일을 클릭했을 때 실행되는 메소드 입니다
    /// 타일은 piece  여부에 따라 동작이  달라집니다
    /// </summary>
    public void OnClickTileButton() {
        _tileClickCount++;
        if (JustBeforDestroyObstacle == null)
        {
            JustBeforDestroyPiece = () => { this.obstacle = null; };
        }
        if (_piece != null)
        {
            //ToDo: 피스가 있을 때 동작
            if (JustBeforDestroyPiece == null)
            {
                JustBeforDestroyPiece = () => { this._piece = null; };
            }
            var needOneClick = GameManager.Instance.SecondTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (needOneClick != null) { 
            
                if (needOneClick.Value.isNeedJustOneClick)
                {
                    isNeedOneClick = true;
                }
            }
            return;
        }

        if (!isNeedOneClick)
        {
            Debug.Log(GameManager.Instance.currentClickedTileindex + " : 클릭한 타일 인덱스");
            var pieceAndCaseValue = GameManager.Instance.FirstTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (pieceAndCaseValue != null)
            {
                var caseValue = pieceAndCaseValue.Value.caseValue;
                if (_piece == null)
                {
                    _piece = pieceAndCaseValue.Value.piece?.GetComponent<Pc>();
                    // 오목이 만들어진 턴에 턴종료를 누르면 종료연출상태로 넘어가는 구문을 다음 스테이트에서 실행하게 함 즉 게임 종료
                    (bool, Pc.Owner) CheckSome = GameManager.Instance._rullManager.CheckGameOver();
                    if (CheckSome.Item1)
                    {
                        GameManager.Instance.finishTurnButton.onClick.RemoveAllListeners();
                        GameManager.Instance.finishTurnButton.onClick.AddListener(() => {
                            GameManager.Instance.GetFSM().ChangeState<FinishDirectionState>(CheckSome.Item2);
                        });
                    }
                }

                switch (caseValue)
                {
                    case -1:
                        Debug.Log(_piece.GetPieceOwner() + "의 말 입니다");
                        ResetClick();
                        break;
                    case 0:
                        cursorImageObj.SetActive(false);
                        ClickedImageObj.SetActive(true);
                        break;
                    case 1:
                        _tileClickCount = 0;
                        Debug.Log("공격종료");
                        break;
                    case 2:
                        _tileClickCount = 0;
                        Debug.Log("금수입니다");
                        break;
                    case 3:
                        Debug.Log("선택종료");
                        ResetClick();
                        break;
                }
            }
        }
        else {
            isNeedOneClick = false;
             _tileClickCount = 0;
        }
      
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (obstacle == null && _piece == null && _tileClickCount == 0)
        cursorImageObj.SetActive(true);
        GameManager.Instance.RangeAttackVisualizeEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorImageObj.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.FirstTimeTileClickEvent == null && GameManager.Instance.SecondTimeTileClickEvent== null) return;
        OnClickTileButton();
    }

    public void ResetTile() {
        cursorImageObj.SetActive(false);
        ClickedImageObj.SetActive(false);
    }

    public void SetPiece(Pc pc) {
        _piece = pc;
    }
}
