using System;
using System.Collections;
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
    public GameObject buffPrefab;
    public Buff Buff;
    public bool isForbiddenMove;
    public Piece Piece { get; set; }
    

    public Action JustBeforeDestroyPieceOrObstacle;

    //타일 초기화는 mc에 있음
    public void TileInit() {
        if (JustBeforeDestroyPieceOrObstacle == null)
        {
            JustBeforeDestroyPieceOrObstacle = () => {
                ResetForbbiden();
            };
        }
    }
    private void ResetPieceObstacle()
    {
        Piece = null;
        obstacle = null;
    }

    public void ResetClick()
    {
        clickedImageObj.SetActive(false);
        _tileClickCount = 0;
    }

    public void ResetClickCount() {
        _tileClickCount = 0;
    }

    public bool IsObstacleNotNull()
    {
        if (obstacle != null)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        return Buff;
    } 
    public void SetBuff(Buff buff)
    {
        this.Buff = buff;
    }

    public void OnClickTileButton() {
        //기보: 클릭 추가
        NotationManager.Instance.AddClickIndex(tileNumber % 8,tileNumber / 8);
        _tileClickCount++;
        // 추가: 타일 클릭 시 HandManager에 선택된 타일 정보 전달
        HandManager hm = FindObjectOfType<HandManager>();
        if (hm != null)
        {
            hm.SetSelectedTile(this);
        }
   

        if (Piece != null)
        {
            var needOneClick = GameManager.Instance.SecondTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (needOneClick != null) { 
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
        else {
            _isNeedOneClick = false;
            _tileClickCount = 0;
        }
    }
    private void ResetForbbiden() {
        ResetPieceObstacle();
        ResetClick();
        Piece.Owner pc = GameManager.Instance.GetCurrentPlayerType();
        if (pc == GameManager.Instance.firstPlayer)
        {
            GameManager.Instance.ruleManager.UpdateForbiddenMoves(pc);
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
            TooltipManager.Instance.CloseTooltip();
            StartCoroutine(ExitPointer());
            return;
        }
        StartCoroutine(ExitPointer());
    }
    IEnumerator ExitPointer() {
        cursorImageObj.SetActive(false);
        yield return null;
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

    public void ResetTile() {
        cursorImageObj.SetActive(false);
        clickedImageObj.SetActive(false);
    }

    public void SetPiece(Piece piece) {
        this.Piece = piece;
    }
}
