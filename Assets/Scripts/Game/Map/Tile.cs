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
    private bool isNeedOneClick;
    public int tileNumber;
    
    public Pc _piece { get; private set; }

    


    /// <summary>
    ///  Ÿ���� obstacle,buff,_piece�� �ʱ�ȭ �ϴ� �޼ҵ� �Դϴ�
    /// </summary>
    public void ResetAll() {
        obstacle = null;
        buff = null;
        _piece = null;
    }

    /// <summary>
    /// Ŭ���� ��Ȳ�� �ʱ�ȭ �ϴ� �޼ҵ� �Դϴ�
    /// </summary>
    public void ResetClick()
    {
        ClickedImageObj.SetActive(false);
        _tileClickCount = 0;
    }

    public Obstacle GetObstacle() { 
            return obstacle;
    }



    /// <summary>
    /// Ÿ���� Ŭ������ �� ����Ǵ� �޼ҵ� �Դϴ�
    /// Ÿ���� piece  ���ο� ���� ������  �޶����ϴ�
    /// </summary>
    public void OnClickTileButton() {
        _tileClickCount++;
        if (_piece != null)
        {
            //ToDo: �ǽ��� ���� �� ����
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
            Debug.Log(GameManager.Instance.currentClickedTileindex + " : Ŭ���� Ÿ�� �ε���");

            var caseValue = pieceAndCaseValue.Value.caseValue;


            if (_piece == null)
            {
                _piece = pieceAndCaseValue.Value.piece?.GetComponent<Pc>();
            }

            switch (caseValue)
            {
                case -1:
                    Debug.Log(_piece.GetPieceOwner() + "�� �� �Դϴ�");
                    ResetClick();
                    break;
                case 0:
                    cursorImageObj.SetActive(false);
                    ClickedImageObj.SetActive(true);
                    break;
                case 1:
                    _tileClickCount = 0;
                    Debug.Log("��������");
                    break;
            }
        }
        else {
            isNeedOneClick = false;
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
