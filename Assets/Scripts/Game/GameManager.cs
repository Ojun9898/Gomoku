using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Piece;
using System.Linq;
using UnityEngine.Serialization;


[RequireComponent(typeof(StateMachine))]
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private MapController mc;
    public GameObject BlockPanelPrefab;
    public GameObject forbiddenMoveObject;
    public GameObject piece;
    public Transform canvasTransform;
    public Button finishTurnButton;
    public GamePanelController gamePanelController;
    public Func<int, int, (GameObject piece, int caseValue)> FirstTimeTileClickEvent;
    public Func<int, int, (bool isNeedJustOneClick, int caseValue)> SecondTimeTileClickEvent;
    public Action RangeAttackVisualizeEvent;
    public Action RangeAttackResetVisualizeEvent;
    public RuleManager ruleManager;
    public int currentClickedTileIndex;
    public HandManager _handManager;
    public List<bool> Costs;
    public int PlayerLevel;
    public CostPanelController cp;
    
    private Owner _playerType;
    private DeckManager _deckManager;
    private int _lastClickedTileIndex = -1;
    private Piece _damagedPiece;
    private Piece _attackingPiece;
    private Piece _currentChoosingPiece;
    private List<int> _currentPieceCanAttackRange;
    private StateMachine _fsm;
    private int _changeTurnCount;



    public MapController Mc { get { return mc; } }
    public int CurrentClickedTileIndex
    {
        get { return currentClickedTileIndex; }

        set
        {
            if (currentClickedTileIndex != value)
            {
                var beforeIndex = currentClickedTileIndex;
                mc.tiles[beforeIndex].ResetClick();
                currentClickedTileIndex = value;
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
        // 턴 넘김 횟수 초기화 
        _changeTurnCount = 0;
        //턴 넘김 버튼 이벤트 추가
        finishTurnButton.onClick.AddListener(OnButtonClickFinishMyTurn);
        // 선공 정하기
        _playerType = SetFirstAttackPlayer();
        // 플레이어 손패 지급
        _handManager = FindObjectOfType<HandManager>();
        _deckManager = FindObjectOfType<DeckManager>();
        _handManager.playerOwner = _playerType;

        _deckManager.InitializeDeck();
        _handManager.InitializeHand(_deckManager);
        //RuleManager 가져오기
        ruleManager = FindAnyObjectByType<RuleManager>();
        //게임패널컨트롤러 가져오기
        gamePanelController = FindAnyObjectByType<GamePanelController>();
        gamePanelController.InitTimer();
        // 코스트 불러오기
        cp = FindObjectOfType<CostPanelController>();
        //RuleManager 초기화
        ruleManager.Init(mc.tiles, _playerType);

        // 상태 머신 가져오기 
        SetFSM();
    }

    private void StartGame()
    {
        _fsm.Run(_playerType);
    }

    /// <summary>
    /// 선공 정하기 메소드
    /// </summary>
    /// <returns></returns>
    public Owner SetFirstAttackPlayer()
    {
        System.Random random = new System.Random();
        int randomIndex = random.Next(1, 3);
        Owner owner = (Owner)1;

        if (Enum.IsDefined(typeof(Owner), randomIndex))
        {
            owner = (Owner)randomIndex;
        }
        return owner;
    }

    public void SetMapController()
    {
        mc = FindAnyObjectByType<MapController>();
        mc.CreateMap();
    }

    public void SetFSM()
    {
        _fsm = GetComponent<StateMachine>();
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
            CurrentClickedTileIndex = tileNumber;
            //처음 클릭 후 
            // 클릭 카운트 2번으로 조건을 두었는데
            // 카드 내기까지 구현이 된다면 클릭 카운트를 1번으로 했을 때 조건에 들어가 카드 내기를 대기하도록

            if (mc.tiles[CurrentClickedTileIndex].isForbiddenMove)
            {
                //금수일때
                return (null, 2);
            }


            if (mc.tiles[CurrentClickedTileIndex].GetObstacle() != null && (_lastClickedTileIndex == -1 || _lastClickedTileIndex == CurrentClickedTileIndex))
            {
                // 장애물 정보 보여주기
                MessageManager.Instance.ShowMessagePanel("장애물이 있습니다");
                return (null, 1);
            }


            if (_lastClickedTileIndex == -1 || mc.tiles[CurrentClickedTileIndex] == null)
            {
                // 기존에는 tileClickCount == 2일 때 말을 생성했으나, 이제 카드를 통해 생성하므로 안내 메시지만 보여줍니다.
                if (tileClickCount == 2)
                {
                    if (_handManager.isAlreadySetPiece)
                    {
                        MessageManager.Instance.ShowMessagePanel("이미 말이 존재합니다");
                        Debug.Log(mc.tiles[CurrentClickedTileIndex].Piece);
                        return (null, 3);
                    }

                    MessageManager.Instance.ShowMessagePanel("카드를 선택해 주세요.");
                    return (null, 0);
                }

                return (null, 0);
            }



            if (_currentPieceCanAttackRange.Contains(CurrentClickedTileIndex) && _currentChoosingPiece != null)
            {
                // 일반 공격, 공격 범위 내에 있을 때
                if (_currentChoosingPiece.attackType == AttackType.CHOOSE_ATTACK && mc.tiles[CurrentClickedTileIndex].GetObstacle() != null)
                { // Todo : 장애물 공격
                    _currentChoosingPiece.ChoseAttack(mc.tiles[CurrentClickedTileIndex].GetObstacle(), _currentChoosingPiece.GetAttackPower());
                    MessageManager.Instance.ShowMessagePanel("장애물을 공격했습니다" + mc.tiles[CurrentClickedTileIndex].GetObstacle().name + "의 Hp:" + mc.tiles[CurrentClickedTileIndex].GetObstacle().Hp);

                }
                else if (_currentChoosingPiece.attackType == AttackType.CHOOSE_ATTACK || _currentChoosingPiece.attackType == AttackType.BUFF)
                {
                    MessageManager.Instance.ShowMessagePanel("공격 대상이 없습니다");
                }
                // 범위 공격, 공격 범위 내에 있을 떄

            }
            else
            {
                MessageManager.Instance.ShowMessagePanel("공격 범위를 벗어납니다");
            }
            // + 장애물 처리
            FinishedAttack();
            return (null, 1);
        };

        SecondTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {
            CurrentClickedTileIndex = tileNumber;

            /* // 범위 공격Piece 공격 범위 보여주기 Todo: 수정 ㄱㄱ
             RangeAttackVisualizeEvent = () =>
             {
                 if (_mc.tiles[currentClickedTileIndex]._piece._attackType == AttackType.RANGE_ATTACK)
                 {
                     var attackPoint = _mc.tiles[currentClickedTileIndex]._piece.RangeAttackCalculate(currentClickedTileIndex);
                     foreach (var point in attackPoint)
                     {
                         _mc.tiles[point].GetComponent<SpriteRenderer>().color = Color.red;
                     }
                 }
             };*/

            if (mc.tiles[CurrentClickedTileIndex].Piece.GetPieceOwner() == _playerType)
            {

                if (_currentPieceCanAttackRange == null)
                {
                    _currentPieceCanAttackRange = CanAttackRangeCalculate(CurrentClickedTileIndex, mc.tiles[CurrentClickedTileIndex].Piece.GetAttackRange());
                    VisualizeAttackRange(_currentPieceCanAttackRange);
                }
                _currentChoosingPiece = mc.tiles[CurrentClickedTileIndex].Piece;




                if (tileClickCount >= 2 && _lastClickedTileIndex == CurrentClickedTileIndex)
                {
                    MessageManager.Instance.ShowMessagePanel("플레이어의 말 입니다");
                    FinishedAttack();
                    return (true, 0);
                }

                if (_lastClickedTileIndex != -1)
                { // 공격턴에 아군 선택 상황
                    _damagedPiece = _currentChoosingPiece;
                    _attackingPiece = mc.tiles[_lastClickedTileIndex].Piece;

                    bool CanAttack = CheckIsLeftCost(Costs,_attackingPiece);

                    if (!CanAttack) {
                        MessageManager.Instance.ShowMessagePanel("코스트가 부족합니다");
                        FinishedAttack();
                        return (true, 0);
                    }

                    if (_currentPieceCanAttackRange.Contains(CurrentClickedTileIndex))
                    {

                        if (_attackingPiece.attackType == AttackType.CHOOSE_ATTACK)
                        {
                            MessageManager.Instance.ShowMessagePanel("아군을 공격할 수 없습니다");
                        }
                        else if (_attackingPiece.attackType == AttackType.RANGE_ATTACK)
                        {
                            MessageManager.Instance.ShowMessagePanel("아군을 공격할 수 없습니다");
                        }
                        else if (_attackingPiece.attackType == AttackType.BUFF)
                        {
                            _attackingPiece.Buff(_damagedPiece, _attackingPiece.GetAttackPower());
                            UseCost(Costs, _attackingPiece);
                            _attackingPiece.animator.Play("ATTACK");
                            _damagedPiece.animator.Play("BUFF");
                            MessageManager.Instance.ShowMessagePanel("아군을 치료했습니다" + "아군의 Hp :" + _damagedPiece.Hp);
                        }
                    }
                    else
                    {
                        MessageManager.Instance.ShowMessagePanel("공격 범위를 벗어납니다");
                    }
                    FinishedAttack();
                    return (true, 0);
                }
                // 나의 말일 때 조건 충족
                // 공격 대기 메소드 실행 단 아직 구현이 안되있으니
                // 임의의 조건문을 사용해 구현 하겠다

                //공격을 하기 위해서는 다른 말을 선택해야하니 공격자의 인덱스를 저장
                _lastClickedTileIndex = CurrentClickedTileIndex;
                if (_currentChoosingPiece.isAlreadyAttack)
                {
                    MessageManager.Instance.ShowMessagePanel("이미 공격한 말 입니다");
                    FinishedAttack();
                    return (true, 0);
                }
                if (_handManager.playerAHandPanel.activeInHierarchy)
                {
                    _handManager.playerAHandPanel.SetActive(false);
                }

                else if (_handManager.playerBHandPanel.activeInHierarchy)
                {
                    _handManager.playerBHandPanel.SetActive(false);
                }
                MessageManager.Instance.ShowMessagePanel("공격할 말을 선택하세요");
            }
            else
            {
                // 적의 말일 때 조건 충족
                // 말의 정보를 보여줌
                if (_lastClickedTileIndex != -1)
                { // 공격턴에 적 선택 상황
                    _damagedPiece = mc.tiles[CurrentClickedTileIndex].Piece;
                    _attackingPiece = mc.tiles[_lastClickedTileIndex].Piece;

                    bool CanAttack = CheckIsLeftCost(Costs, _attackingPiece);

                    if (!CanAttack)
                    {
                        MessageManager.Instance.ShowMessagePanel("코스트가 부족합니다");
                        FinishedAttack();
                        return (true, 0);
                    }

                    if (_currentPieceCanAttackRange.Contains(CurrentClickedTileIndex))
                    {
                        if (_attackingPiece.attackType == AttackType.CHOOSE_ATTACK)
                        {
                            _attackingPiece.ChoseAttack(_damagedPiece, _attackingPiece.GetAttackPower());
                            UseCost(Costs, _attackingPiece);
                            MessageManager.Instance.ShowMessagePanel("적을 공격했습니다" + "남은 HP : " + _damagedPiece.Hp);
                            _attackingPiece.animator.Play("ATTACK");
                            _damagedPiece.animator.Play("DAMAGED");
                            if (_damagedPiece.hp <= 0)
                            {
                                _damagedPiece.animator.Play("DEATH");
                            }
                        }
                        else if (_attackingPiece.attackType == AttackType.RANGE_ATTACK)
                        {
                            //attackingPiece.RangeAttack(currentClickedTileIndex);
                            UseCost(Costs, _attackingPiece);
                            MessageManager.Instance.ShowMessagePanel("적을 공격했습니다" + "남은 HP : " + _damagedPiece.Hp);
                            _attackingPiece.animator.Play("ATTACK");
                            _damagedPiece.animator.Play("DAMAGED");
                            if (_damagedPiece.hp <= 0)
                            {
                                _damagedPiece.animator.Play("DEATH");
                            }
                        }
                        else if (_attackingPiece.attackType == AttackType.BUFF)
                        {
                            MessageManager.Instance.ShowMessagePanel("적에게 버프를 줄 수 없습니다");
                        }
                    }
                    else
                    {
                        MessageManager.Instance.ShowMessagePanel("공격 범위를 벗어납니다");

                    }
                    FinishedAttack();
                }
                else
                {
                    MessageManager.Instance.ShowMessagePanel("상대의 말입니다");
                    return (true, 0);
                }
            }
            return (true, 0);
        };
    }

    private bool CheckIsLeftCost(List<bool> costs,Piece comparedPiece) { 

        int trueCount = 0;
        int attackCost = comparedPiece.cost;

        for (int i = 0; i < costs.Count; i++)
        {
            if (costs[i] == true)
            {
                trueCount++;
            }
        }
        PrintBoolList(costs);
        if (attackCost <= trueCount)
        {
            return true;
        }
        else {
            return false;
        }
    }

    public static void PrintBoolList(List<bool> boolList)
    {
        // 1. 리스트의 내용을 0(false)과 1(true)로 변환하여 출력
        Debug.Log("리스트 상태: " + string.Join(" ", boolList.ConvertAll(b => b ? "1" : "0")));
    }

    private void UseCost(List<bool> costs, Piece comparedPiece) {
        int attackCost = comparedPiece.cost;
        int firstTrueIndex = 0;
        for (int i = 0; i < costs.Count; i++)
        {
            if (costs[i]) {
                firstTrueIndex = i;
                break;
            }
        }

        for (int i = firstTrueIndex; i < firstTrueIndex + attackCost; i++)
        {
            costs[i] = false;
        }
        
        cp.SetCost(costs);
    }




    /// <summary>
    /// Ai 턴에 공격을 하지 못하도록 타일 입력을 막는 메소드
    /// </summary>
    public void SetTileClickEventOff()
    {
        FinishedAttack();
        FirstTimeTileClickEvent = null;
        SecondTimeTileClickEvent = null;
    }


    /// <summary>
    /// 턴이 끝나면 부를 메소드 피스를 하나라도 두었는지의 유무를 초기화합니다
    /// </summary>
    public void SetFalseIsAlreadySetPiece()
    {
        _handManager.isAlreadySetPiece = false;
    }

    /// <summary>
    /// 공격 상황이 끝났을 때를 가정하고 모든 상황을 초기화하는 메소드
    /// </summary>
    private void FinishedAttack()
    {
        _damagedPiece = null;
        _attackingPiece = null;
        RangeAttackVisualizeEvent = null;
        _lastClickedTileIndex = -1;
        mc.tiles[currentClickedTileIndex]?.ResetTile();
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
    public GameObject SetPieceAtTile(int tileIndex)
    {
        var pieceInstance = Instantiate(this.piece, mc.tiles[tileIndex].transform);
        pieceInstance.transform.position += Vector3.down * 0.31f;
        return pieceInstance;
    }


    /// <summary>
    /// 임시  Piece를 만들어 주는 메소드
    /// </summary>
    /// <param name="index"></param>
    /// <param name="currentPlayer"></param>
    /// <returns>만든 Piece</returns>
    public Piece SetTemporaryPiece(int index, Owner currentPlayer)
    {

        Piece pieceInstance = Instantiate(this.piece).GetComponent<Piece>();
        if (currentPlayer == Owner.PLAYER_B)
        {
            pieceInstance.pieceOwner = Owner.PLAYER_B;
        }
        else if (currentPlayer == Owner.PLAYER_A)
        {
            pieceInstance.pieceOwner = Owner.PLAYER_A;
        }
        return pieceInstance;
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
            mc.tiles[index].rangeImageObj.SetActive(true);
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
            mc.tiles[index].rangeImageObj.SetActive(false);
        }
        attackRange = null;
    }

    public void OnButtonClickFinishMyTurn()
    {
        (bool, Piece.Owner) CheckSome = GameManager.Instance.ruleManager.CheckGameOver();
        if (CheckSome.Item1)
        {
            finishTurnButton.onClick.RemoveAllListeners();
            finishTurnButton.onClick.AddListener(() =>
            {
                GetFSM().ChangeState<FinishDirectionState>(CheckSome.Item2);
            });
            finishTurnButton.onClick.Invoke();
        }

        if (_handManager.isAlreadySetPiece)
        {
            GameManager.Instance.gamePanelController.StopTimer();
            _changeTurnCount++;
            Debug.Log("턴 진행 횟수 : " + _changeTurnCount);
            if (_changeTurnCount >= 30)
            {
                //우승자 넘기기
                _fsm.ChangeState<FinishDirectionState>(ruleManager.NotFinishedOnPlayingGame());
                return;
            }

            switch (_playerType)
            {
                case Owner.PLAYER_A:
                    _playerType = Owner.PLAYER_B;
                    _handManager.playerOwner = _playerType;
                    _handManager.isAlreadySetPiece = false;
                    _handManager.playerAHandPanel.SetActive(false);
                    
                    if (_handManager.playerAHandCards != null)
                    {
                        DeckManager.Card card = _deckManager.PopCard(_handManager.GetPlayerDeck(_deckManager.playerACards));
                        if (card != null)
                        {
                            _handManager.playerAHandCards.Add(card);
                            _handManager.CreateCardUI(card, Owner.PLAYER_A);
                        }
                    } 
                    
                    break;
                case Owner.PLAYER_B:
                    _playerType = Owner.PLAYER_A;
                    _handManager.playerOwner = _playerType;
                    _handManager.isAlreadySetPiece = false;
                    _handManager.playerBHandPanel.SetActive(false);
                    
                    if (_handManager.playerBHandCards != null)
                    {
                        DeckManager.Card card = _deckManager.PopCard(_handManager.GetPlayerDeck(_deckManager.playerBCards));
                        if (card != null)
                        {
                            _handManager.playerBHandCards.Add(card);
                            _handManager.CreateCardUI(card, Owner.PLAYER_B);
                        }
                    } 
                    
                    break;
            }

            for (int i = 0; i < mc.tiles.Count; i++)
            {
                if (mc.tiles[i].Piece != null)
                {
                    mc.tiles[i].Piece.isAlreadyAttack = false;
                }
            }


            FinishedAttack();
           
            if (_playerType == Owner.PLAYER_B)
            {
                _fsm.ChangeState<AITurnState>(_playerType);
            }
            else
            {
                _fsm.ChangeState<PlayerTurnState>(_playerType);
            }
        }
        else
        {
            MessageManager.Instance.ShowMessagePanel("말을 놓아주세요");
        }
    }

    /// <summary>
    ///  맵위 모든 piece의 공격 초기화 메소드
    /// </summary>
    public void PiecesInit()
    {
        var indices = mc.tiles.Select((tile, idx) => new { Tile = tile, Index = idx })  // Tile과 해당 인덱스를 함께 반환
           .Where(x => x.Tile.Piece != null)  // Piece가 있는 Tile만 필터링
           .Select(x => x.Index)  // 인덱스만 추출
           .ToList();  // 결과를 리스트로 반환

        foreach (var index in indices)
        {
            mc.tiles[index].Piece.isAlreadyAttack = false;
        }
    }

    public void AllTileClickCountSetZero()
    {
        for (int i = 0; i < mc.tiles.Count; i++)
        {
            mc.tiles[i].ResetClick();
        }
    }

    public Owner GetCurrentPlayerType()
    {
        return _playerType;
    }

    public bool GetIsAlReadySetPiece()
    {
        return _handManager.isAlreadySetPiece;
    }
    public void SetIsAlReadySetPiece(bool isAlreadySetPiece)
    {
        _handManager.isAlreadySetPiece = isAlreadySetPiece;
    }

    public StateMachine GetFSM()
    {
        return _fsm;
    }
}
