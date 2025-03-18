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
    private Buff _buff;
    public Pc _piece { get; private set; }

    


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
        if (_piece != null)
        {
            //ToDo: 피스가 있을 때 동작
            var needOneClick = GameManager.Instance.SecondTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (needOneClick.Value.isNeedJustOneClick)
            {
                isNeedOneClick = true;
            }
            return;
        }

        if (!isNeedOneClick)
        {
            var pieceAndCaseValue = GameManager.Instance.FirstTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (pieceAndCaseValue == null) { _tileClickCount = 0; return; }
            Debug.Log(GameManager.Instance.currentClickedTileindex + " : 클릭한 타일 인덱스");

            var caseValue = pieceAndCaseValue.Value.caseValue;


            if (_piece == null)
            {
                _piece = pieceAndCaseValue.Value.piece?.GetComponent<Pc>();
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
            }
        }
        else {
            isNeedOneClick = false;
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
        OnClickTileButton();
    }
}
