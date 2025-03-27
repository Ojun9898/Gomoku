using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject cursorImageObj;
    public GameObject clickedImageObj;
    private int _tileClickCount;
    private bool _isNeedOneClick;
    public int tileNumber;

    [SerializeField] private Obstacle obstacle;
    public bool isForbiddenMove;
    private Buff _buff;
    public Piece Piece { get; set; }
    
    private HandManager _handManager;

    public Action JustBeforeDestroyPiece;
    public Action JustBeforeDestroyObstacle;

    private void Start()
    {
        _handManager = FindObjectOfType<HandManager>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // UI 요소 위를 클릭한 경우는 제외하고
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // 마우스 위치에서 Raycast를 실행하여 Tile 컴포넌트가 있는지 확인
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (!Physics.Raycast(ray, out hit))
                {
                        _handManager.playerAHandPanel.SetActive(false);
                        _handManager.playerBHandPanel.SetActive(false);
                }
                else
                {
                    // 만약 Raycast된 오브젝트에 Tile 컴포넌트가 없다면 카드 패널 비활성화
                    if (hit.collider.GetComponent<Tile>() == null)
                    {
                        _handManager.playerAHandPanel.SetActive(false);
                        _handManager.playerBHandPanel.SetActive(false);
                    }
                }
            }
        }
    }

    public void ResetAll() {
        obstacle = null;
        _buff = null;
        Piece = null;
    }

    public void ResetClick()
    {
        clickedImageObj.SetActive(false);
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

    public void OnClickTileButton() {
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
            if (_handManager.playerOwner == Piece.Owner.PLAYER_A)
            {
                _handManager.playerAHandPanel.SetActive(true);
            }
            else
            {
                _handManager.playerBHandPanel.SetActive(true);
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
        else {
            _isNeedOneClick = false;
            _tileClickCount = 0;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (obstacle == null && Piece == null && _tileClickCount == 0)
            cursorImageObj.SetActive(true);
        GameManager.Instance.RangeAttackVisualizeEvent?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cursorImageObj.SetActive(false);
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
