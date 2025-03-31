using System;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject cursorImageObj;
    public GameObject clickedImageObj;
    public GameObject rangeImageObj;
    private int _tileClickCount;
    private bool _isNeedOneClick;
    public int tileNumber;
    public int hp = 0;
     private bool isTooltipActive = false;

    [SerializeField] private Obstacle obstacle;
    public bool isForbiddenMove;
    private Buff _buff;
    public Piece Piece { get; set; }


    public Action JustBeforeDestroyPiece;
    public Action JustBeforeDestroyObstacle;


    public void ResetAll()
    {
        obstacle = null;
        _buff = null;
        Piece = null;
    }

    public void ResetClick()
    {
        clickedImageObj.SetActive(false);
        _tileClickCount = 0;
    }

    public void ResetClickCount()
    {
        _tileClickCount = 0;
    }

    public Obstacle GetObstacle()
    {
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

    public void OnClickTileButton()
    {
        _tileClickCount++;
        // 추가: 타일 클릭 시 HandManager에 선택된 타일 정보 전달
        HandManager hm = FindObjectOfType<HandManager>();
        if (hm != null)
        {
            hm.SetSelectedTile(this);
        }

        if (JustBeforeDestroyObstacle == null)
        {
            JustBeforeDestroyPiece = () => { this.obstacle = null; };
        }
        if (Piece != null)
        {
            if (JustBeforeDestroyPiece == null)
            {
                JustBeforeDestroyPiece = () => { this.Piece = null; };
            }
            var needOneClick = GameManager.Instance.SecondTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (needOneClick != null)
            {
                if (needOneClick.Value.isNeedJustOneClick)
                {
                    _isNeedOneClick = true;
                }
            }
            return;
        }

        if (!_isNeedOneClick)
        {
            Debug.Log(GameManager.Instance.CurrentClickedTileIndex + " : 클릭한 타일 인덱스");
            if (GameManager.Instance._handManager.playerOwner == Piece.Owner.PLAYER_A)
            {
                GameManager.Instance._handManager.playerAHandPanel.SetActive(true);
            }
            else
            {
                GameManager.Instance._handManager.playerBHandPanel.SetActive(true);
            }
            var pieceAndCaseValue = GameManager.Instance.FirstTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (pieceAndCaseValue != null)
            {
                var caseValue = pieceAndCaseValue.Value.caseValue;
                if (Piece == null)
                {
                    Piece = pieceAndCaseValue.Value.piece?.GetComponent<Piece>();
                }

                switch (caseValue)
                {
                    case -1:
                        Debug.Log(Piece.GetPieceOwner() + "의 말 입니다");
                        ResetClick();
                        break;
                    case 0:
                        cursorImageObj.SetActive(false);
                        clickedImageObj.SetActive(true);
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
        else
        {
            _isNeedOneClick = false;
            _tileClickCount = 0;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        if (obstacle == null && Piece == null && _tileClickCount == 0)
            cursorImageObj.SetActive(true);

        if (Piece != null)
        {
            TooltipManager.Instance.ShowTooltip("HP: " + Piece.Hp);
            isTooltipActive = true;
        }

        if (obstacle != null)
        {
            TooltipManager.Instance.ShowTooltip("HP: " + obstacle.Hp);
            isTooltipActive = true;
        }

        GameManager.Instance.RangeAttackVisualizeEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTooltipActive)
        {
            // 툴팁이 활성화된 상태에서만 종료하도록 방지
            return;
        }

        cursorImageObj.SetActive(false);
        TooltipManager.Instance.CloseTooltip();
    }

    public void CloseTooltipIfActive()
    {
        if (isTooltipActive)
        {
            TooltipManager.Instance.CloseTooltip();
            isTooltipActive = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.FirstTimeTileClickEvent == null && GameManager.Instance.SecondTimeTileClickEvent == null) return;
        OnClickTileButton();
    }

    public void ResetTile()
    {
        cursorImageObj.SetActive(false);
        clickedImageObj.SetActive(false);
    }

    public void SetPiece(Piece piece)
    {
        this.Piece = piece;
    }
}
