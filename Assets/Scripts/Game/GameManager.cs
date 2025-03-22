using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Pc;
using System.Linq;


[RequireComponent(typeof(StateMachine))]
public class GameManager : Singleton<GameManager>
{

    [SerializeField] private MapController _mc;
    [SerializeField] private GameObject piece;

    public Button finishTurnButton;
    public Func<int, int, (GameObject piece, int caseValue)> FirstTimeTileClickEvent;
    public Func<int, int, (bool isNeedJustOneClick, int caseValue)> SecondTimeTileClickEvent;
    public Action RangeAttackVisualizeEvent;
    public Action RangeAttackResetVisualizeEvent;


    private Pc.Owner _playerType;
    private int _currentClickedTileindex;
    private int _lastClickedTileindex = -1;
    private Pc _damagedPiece;
    private Pc _attackingPiece;
    private Pc _currentChoosingPiece;
    private List<int> _currentPieceCanAttackRange;
    private bool IsAleadySetPiece;
    private StateMachine _FSM;
    private int ChangeTurnCount;



    public MapController Mc { get { return _mc; } }
    public int currentClickedTileindex
    {
        get { return _currentClickedTileindex; }

        set
        {
            if (_currentClickedTileindex != value)
            {
                var beforIndex = _currentClickedTileindex;
                _mc.tiles[beforIndex].ResetClick();
                _currentClickedTileindex = value;
            }
        }
    }



    private void Awake()
    {
        InitGameManager();
  
        StartGame();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }

    /// <summary>
    /// 게임 메니저 초기화
    /// </summary>
    private void InitGameManager()
    {   // 카드 생성 메소드 

        // Map에서 타일 생성후 가져오는 메소드
        SetMapController();
        // 기타 초기화 
        ChangeTurnCount = 0;
        //턴 넘김 버튼 이벤트 추가
        finishTurnButton.onClick.AddListener(OnButtonClickFinishMyTurn);
        // 선공 정하기
        _playerType =  SetFirstAttackPlayer();
        // 상태 머신 가져오기 
        SetFSM();
    }

    private void StartGame()
    {
        _FSM.Run(_playerType);
    }

    /// <summary>
    /// 선공 정하기 메소드
    /// </summary>
    /// <returns></returns>
    public Pc.Owner SetFirstAttackPlayer() {
        System.Random random = new System.Random();
        int randomIndex = random.Next(1, 3);
        Pc.Owner owner = (Pc.Owner)1;

        if (Enum.IsDefined(typeof(Pc.Owner), randomIndex))
        {
            owner = (Pc.Owner)randomIndex;
        }
        return owner;
    }

    public void SetMapController() {
        _mc = FindAnyObjectByType<MapController>();
    }

    public void SetFSM()
    {
        _FSM = GetComponent<StateMachine>();
    }


    //카드 내기 대기 메소드
    //말 공격 대기 메소드

    /// <summary>
    /// 타일 클릭 설정을 부여하는 메소드
    /// </summary>
    public void SetTileClickEvent()
    {
        FirstTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {       
            currentClickedTileindex = tileNumber;
            //처음 클릭 후 
            // 클릭 카운트 2번으로 조건을 두었는데
            // 카드 내기까지 구현이 된다면 클릭 카운트를 1번으로 했을 때 조건에 들어가 카드 내기를 대기하도록



            if (_mc.tiles[currentClickedTileindex].GetObstacle() != null && (_lastClickedTileindex == -1 || _lastClickedTileindex == currentClickedTileindex))
            {
                // 장애물 정보 보여주기
                Debug.Log("장애물이 있습니다");
                return (null, 1);
            }


            if (_lastClickedTileindex == -1 || _mc.tiles[currentClickedTileindex] == null)
            {

                if (tileClickCount == 2)
                {
                    if (IsAleadySetPiece) {
                        Debug.Log("이미 말을 놓았습니다");
                        return (null, 0);
                    }
                    var _piece = SetPieceAtTile(currentClickedTileindex);
                    IsAleadySetPiece = true;
                    var pc = _piece.GetComponent<Pc>();
                    pc?.SetPieceOwner(_playerType);
                    if (_playerType == Owner.PLAYER_B) {
                        pc.GetComponent<SpriteRenderer>().color = Color.black;
                    }

                    if (_mc.tiles[currentClickedTileindex].GetBuff() != null)
                    {
                        //Todo:버프 활성화?
                        _mc.tiles[currentClickedTileindex].GetBuff().On(_piece.GetComponent<Pc>());
                        //_mc.ActiveBuff(_mc.tiles[currentClickedTileindex]);
                    }
                    return (_piece, -1);
                }
                return (null, 0);
            }



            if (_currentPieceCanAttackRange.Contains(currentClickedTileindex) && _currentChoosingPiece != null)
            {
                // 일반 공격, 공격 범위 내에 있을 때
                if (_currentChoosingPiece._attackType == AttackType.CHOOSE_ATTACK && _mc.tiles[currentClickedTileindex].GetObstacle() != null)
                { // Todo : 장애물 공격
                    _currentChoosingPiece.ChoseAttack(_mc.tiles[currentClickedTileindex].GetObstacle(), _currentChoosingPiece.GetAttackPower());
                    Debug.Log("장애물을 공격했습니다" + _mc.tiles[currentClickedTileindex].GetObstacle().name + "의 Hp:" + _mc.tiles[currentClickedTileindex].GetObstacle().Hp);

                }
                else if (_currentChoosingPiece._attackType == AttackType.CHOOSE_ATTACK || _currentChoosingPiece._attackType == AttackType.BUFF)
                {
                    Debug.Log("공격 대상이 없습니다");
                }
                // 범위 공격, 공격 범위 내에 있을 떄

            }
            else
            {
                Debug.Log("공격범위 외 입니다");
            }
            // + 장애물 처리
            FinishiedAttack();
            return (null, 1);
        };

        SecondTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {
            currentClickedTileindex = tileNumber;

            /* // 범위 공격Piece 공격 범위 보여주기 Todo: 수정 ㄱㄱ
             RangeAttackVisualizeEvent = () =>
             {
                 if (_mc.tiles[currentClickedTileindex]._piece._attackType == AttackType.RANGE_ATTACK)
                 {
                     var attackPoint = _mc.tiles[currentClickedTileindex]._piece.RangeAttackCalculate(currentClickedTileindex);
                     foreach (var point in attackPoint)
                     {
                         _mc.tiles[point].GetComponent<SpriteRenderer>().color = Color.red;
                     }
                 }
             };*/

            if (_mc.tiles[currentClickedTileindex]._piece.GetPieceOwner() == _playerType)
            {
                
                if (_currentPieceCanAttackRange == null)
                {
                    _currentPieceCanAttackRange = CanAttackRangeCalculate(currentClickedTileindex, _mc.tiles[currentClickedTileindex]._piece.GetAttackRange());
                    VisualizeAttackRange(_currentPieceCanAttackRange);
                }
                _currentChoosingPiece = _mc.tiles[currentClickedTileindex]._piece;

    


                    if (tileClickCount >= 2 && _lastClickedTileindex == currentClickedTileindex)
                {
                    Debug.Log("자신의 말을  골랐습니다");
                    FinishiedAttack();
                    return (true, 0);
                }

                if (_lastClickedTileindex != -1)
                { // 공격턴에 아군 선택 상황
                    _damagedPiece = _currentChoosingPiece;
                    _attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;

                    

                    if (_currentPieceCanAttackRange.Contains(currentClickedTileindex))
                    {

                        if (_attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                        {
                            Debug.Log("아군을 직접적으로 공격할 수 없습니다");
                        }
                        else if (_attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                        {
                            Debug.Log("아군을 직접적으로 공격할 수 없습니다");
                        }
                        else if (_attackingPiece._attackType == Pc.AttackType.BUFF)
                        {
                            _attackingPiece.Buff(_damagedPiece, _attackingPiece.GetAttackPower());
                            Debug.Log("아군을 치료했습니다" + _damagedPiece.name + "의 Hp:" + _damagedPiece.Hp);
                        }
                    }
                    else
                    {
                        Debug.Log("공격범위 외 입니다");
                    }
                    FinishiedAttack();
                    return (true, 0);
                }
                // 나의 말일 때 조건 충족
                // 공격 대기 메소드 실행 단 아직 구현이 안되있으니
                // 임의의 조건문을 사용해 구현 하겠다

                //공격을 하기 위해서는 다른 말을 선택해야하니 공격자의 인덱스를 저장
                _lastClickedTileindex = currentClickedTileindex;
                if (_currentChoosingPiece.IsAleayAttack)
                {
                    Debug.Log("이미 기능을 시전했습니다");
                    FinishiedAttack();
                    return (true, 0);
                }
                Debug.Log("공격할 말을 선택하세요" + _lastClickedTileindex);
            }
            else
            {
                // 적의 말일 때 조건 충족
                // 말의 정보를 보여줌
                if (_lastClickedTileindex != -1)
                { // 공격턴에 적 선택 상황
                    _damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    _attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;
                    if (_currentPieceCanAttackRange.Contains(currentClickedTileindex))
                    {
                        if (_attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                        {
                            _attackingPiece.ChoseAttack(_damagedPiece, _attackingPiece.GetAttackPower());
                            Debug.Log("적을 공격했습니다" + _damagedPiece.name + "의 Hp:" + _damagedPiece.Hp);

                        }
                        else if (_attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                        {
                            //attackingPiece.RangeAttack(currentClickedTileindex);
                            Debug.Log("적을 공격했습니다" + _damagedPiece.name + "의 Hp:" + _damagedPiece.Hp);

                        }
                        else if (_attackingPiece._attackType == Pc.AttackType.BUFF)
                        {
                            Debug.Log("적에게 버프를 줄 수 없습니다");
                        }
                    }
                    else
                    {
                        Debug.Log("공격범위 외 입니다");
                    }
                    FinishiedAttack();
                }
                else
                {
                    Debug.Log("적의 말 입니다");
                    return (true, 0);
                }
            }
            return (true, 0);
        };
    }

    /// <summary>
    /// Ai 턴에 공격을 하지 못하도록 타일 입력을 막는 메소드
    /// </summary>
    public void SetTileClickEventOff() {
        FinishiedAttack();
        FirstTimeTileClickEvent = null;
         SecondTimeTileClickEvent = null;
    }


    /// <summary>
    /// 턴이 끝나면 부를 메소드 피스를 하나라도 두었는지의 유무를 초기화합니다
    /// </summary>
    public void SetFalseIsAleadySetPiece() {
        IsAleadySetPiece = false;
    }

    /// <summary>
    /// 공격 상황이 끝났을 때를 가정하고 모든 상황을 초기화하는 메소드
    /// </summary>
    private void FinishiedAttack()
    {
        _damagedPiece = null;
        _attackingPiece = null;
        RangeAttackVisualizeEvent = null;
        _lastClickedTileindex = -1;
        _mc.tiles[_currentClickedTileindex]?.ResetTile(); 
        if (_currentPieceCanAttackRange != null)
        {
            ResetVisualizeAttackRange(ref _currentPieceCanAttackRange);
        }
    }

    /// <summary>
    /// Piece를 Tile에 설치하는 메소드
    /// </summary>
    /// <param name="tileIndex">타일 인덱스 </param>
    /// <returns></returns>
    public GameObject SetPieceAtTile(int tileIndex) {
       return Instantiate(this.piece, _mc.tiles[tileIndex].transform);
    }


    /// <summary>
    /// Piece의 공격 가능 범위를 계산하는 메소드 
    /// 밑의 VisualizeAttackRange, ResetVisualizeAttackRange 와 함께 MapController로 이동필요
    /// </summary>
    /// <param name="index"> 선택한 타일 위치</param>
    /// <param name="attackRange">piece의 사거리</param>
    /// <returns> piece의 Range에 따른 공격 가능 범위</returns>
    public List<int> CanAttackRangeCalculate(int index, int attackRange)
    {
        int width = 8;
        int height = 8;

        int y = index / 8;
        int x = index % 8;

        List<int> result = new List<int>();

        // 상하좌우 1칸 범위 내에서 공격할 요소를 출력
        for (int dy = -attackRange; dy <= attackRange; dy++)
        {
            for (int dx = -attackRange; dx <= attackRange; dx++)
            {
                int targetX = x + dx;
                int targetY = y + dy;

                // 배열의 범위 내에 있는지 체크
                if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                {
                    // 자신을 제외하려면 (x, y) 좌표는 건너뛰기
                    if (targetX == x && targetY == y)
                    {
                        continue; // 자신을 제외
                    }

                    // 1D 배열로 2D 위치 접근
                    int indexs = targetY * width + targetX;
                    result.Add(indexs);
                }
            }
        }
        return result;
    }
    /// <summary>
    /// piece의 공격 가능 범위를 타일에 시각화 합니다
    /// </summary>
    /// <param name="attackRange">CanAttackRangeCalculate 에서 반환 된 값</param>
    private void VisualizeAttackRange(List<int> attackRange)
    {
        foreach (var index in attackRange)
        {
            _mc.tiles[index].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    /// <summary>
    /// 공격 범위 시각화를 초기화 합니다 + _currentPieceCanAttackRange 초기화
    /// </summary>
    /// <param name="attackRange">CanAttackRangeCalculate 에서 반환 된 값</param>
    private void ResetVisualizeAttackRange(ref List<int> attackRange)
    {
        foreach (var index in attackRange)
        {
            _mc.tiles[index].GetComponent<SpriteRenderer>().color = Color.white;
        }
        attackRange = null;
    }

    public void OnButtonClickFinishMyTurn() {
        if (IsAleadySetPiece)
        {
            ChangeTurnCount++;
            Debug.Log("턴 진행 횟수 : "+ ChangeTurnCount);
            if (ChangeTurnCount >= 4) { 
                //우승자 넘기기
                _FSM.ChangeState<FinishDirectionState>(_playerType);
                return;
            }

            switch (_playerType)
            {
                case Pc.Owner.PLAYER_A:
                    _playerType = Pc.Owner.PLAYER_B;
                    break;
                case Pc.Owner.PLAYER_B:
                    _playerType = Pc.Owner.PLAYER_A;
                    break;
            }
            FinishiedAttack();
            //AI 로 가정
            //일단 버튼은 둘다 누를 수 있게 해둠
            if (_playerType == Pc.Owner.PLAYER_B)
            {
                //타일 off
                GameManager.Instance.SetTileClickEventOff();
                _FSM.ChangeState<AITurnState>(_playerType);
            }
            else
            {
                _FSM.ChangeState<PlayerTurnState>(_playerType);
            }
        }
        else { 
            Debug.Log("말을 놓아주세요");
        }
    }


    /// <summary>
    ///  맵위 모든 piece의 공격 초기화 메소드
    /// </summary>
    public void PieceSIni1t()
    {
        var indices = _mc.tiles.Select((tile, idx) => new { Tile = tile, Index = idx })  // Tile과 해당 인덱스를 함께 반환
           .Where(x => x.Tile._piece != null)  // Piece가 있는 Tile만 필터링
           .Select(x => x.Index)  // 인덱스만 추출
           .ToList();  // 결과를 리스트로 반환

        foreach (var index in indices) { 
                _mc.tiles[index]._piece.IsAleayAttack = false;
        }
    }
}
