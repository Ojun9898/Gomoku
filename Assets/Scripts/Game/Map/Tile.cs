using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour , IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [SerializeField] private Obstacle obstacle;
    [SerializeField] private Buff buff;
    [SerializeField] private GameObject cursorImageObj;
    [SerializeField] private GameObject ClickedImageObj;
    private int _tileClickCount;

    public int tileNumber;
    
    public Pc _piece { get; private set; }

    


    /// <summary>
    ///  타일의 obstacle,buff,_piece를 초기화 하는 메소드 입니다
    /// </summary>
    public void ResetAll() {
        obstacle = null;
        buff = null;
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

    /// <summary>
    /// 타일을 클릭했을 때 실행되는 메소드 입니다
    /// 타일은 piece  여부에 따라 동작이  달라집니다
    /// </summary>
    public void OnClickTileButton() {
        _tileClickCount++;
        if (_piece != null)
        {
            //ToDo: 피스가 있을 때 동작
            return;
        }
        else {
            var pieceAndCaseValue =  GameManager.Instance.FirstTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            Debug.Log(GameManager.Instance.currentClickedTileindex +" : 클릭한 타일 인덱스");
        
            _piece =  pieceAndCaseValue.Value.piece?.GetComponent<Pc>();
            var caseValue = pieceAndCaseValue.Value.caseValue;
            if (caseValue == -1)
            {
                ClickedImageObj.SetActive(false);
                Debug.Log(_piece.GetPieceOwner() + "의 말 입니다");
                _tileClickCount = 0;
            }
            else if (caseValue == 0) {
                cursorImageObj.SetActive(false);
                ClickedImageObj.SetActive(true);
            }            
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_piece == null && _tileClickCount == 0)
        cursorImageObj.SetActive(true);
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
