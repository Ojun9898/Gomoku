using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private GameObject cursorImageObj;
    [SerializeField] private GameObject ClickedImageObj;
    [SerializeField] private GameObject handPanelPrefab;
    private int _tileClickCount;
    private bool isNeedOneClick;
    public int tileNumber;

    [SerializeField] private Obstacle obstacle;
    public bool isForbiddenMove;
    private Buff _buff;
    public Pc _piece { get; private set; }

    public Action JustBeforDestroyPiece;
    public Action JustBeforDestroyObstacle;

    private void Start()
    {
        handPanelPrefab = FindObjectOfType<HandManager>().transform.GetChild(0).gameObject;
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
                    handPanelPrefab.SetActive(false);
                }
                else
                {
                    // 만약 Raycast된 오브젝트에 Tile 컴포넌트가 없다면 카드 패널 비활성화
                    if (hit.collider.GetComponent<Tile>() == null)
                    {
                        handPanelPrefab.SetActive(false);
                    }
                }
            }
        }
    }

    public void ResetAll() {
        obstacle = null;
        _buff = null;
        _piece = null;
    }

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

    public void OnClickTileButton() {
        _tileClickCount++;
        if (JustBeforDestroyObstacle == null)
        {
            JustBeforDestroyPiece = () => { this.obstacle = null; };
        }
        if (_piece != null)
        {
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
            handPanelPrefab.SetActive(true);
            var pieceAndCaseValue = GameManager.Instance.FirstTimeTileClickEvent?.Invoke(tileNumber, _tileClickCount);
            if (pieceAndCaseValue != null)
            {
                var caseValue = pieceAndCaseValue.Value.caseValue;
                if (_piece == null)
                {
                    _piece = pieceAndCaseValue.Value.piece?.GetComponent<Pc>();
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
        if (GameManager.Instance.FirstTimeTileClickEvent == null && GameManager.Instance.SecondTimeTileClickEvent == null) return;
        OnClickTileButton();
        // 추가: 타일 클릭 시 HandManager에 선택된 타일 정보 전달
        HandManager hm = FindObjectOfType<HandManager>();
        if (hm != null)
        {
            hm.SetSelectedTile(this);        
        }
        
    }

    public void ResetTile() {
        cursorImageObj.SetActive(false);
        ClickedImageObj.SetActive(false);
    }

    public void SetPiece(Pc pc) {
        _piece = pc;
    }
}
