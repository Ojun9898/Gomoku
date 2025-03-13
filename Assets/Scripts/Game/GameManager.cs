using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : Singleton<GameManager>
{    
    [SerializeField] private Mc _mc;
    [SerializeField] private GameObject piece;

    public Func<int, int, (GameObject piece, int caseValue)> FirstTimeTileClickEvent;
    public Func<int, int, (bool isNeedJustOneClick,int caseValue)> SecondTimeTileClickEvent;


    // 런타임 도중에 계속 변화하지만 일단은 지정해 뒀음
    private Pc.Owner _playerType = Pc.Owner.PLAYER_A;
    private int _currentClickedTileindex;
    private int _lastClickedTileindex = -1;
    
    public int currentClickedTileindex
    {
        get { return _currentClickedTileindex; }

        set
        {
            if (_currentClickedTileindex != value) {
                var beforIndex = _currentClickedTileindex;
                _mc.tiles[beforIndex].ResetClick();
                _currentClickedTileindex = value;
            }

        }
    }
    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    private void Awake()
    {
        InitGameManager();
    }

    /// <summary>
    /// 게임 메니저 초기화
    /// </summary>
    private void InitGameManager()
    {
        // Map에서 타일 생성후 가져오는 메소드
        // piece 생성후 가져오는 메소드
        // 카드 생성 메소드 
        SetTileClickEvent();
    }

    //카드 내기 대기 메소드
    //말 공격 대기 메소드

    /// <summary>
    /// 타일 클릭 설정을 부여하는 메소드
    /// </summary>
    private void SetTileClickEvent()
    {
        FirstTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {
            currentClickedTileindex = tileNumber;
            //처음 클릭 후 
            // 클릭 카운트 2번으로 조건을 두었는데
            // 카드 내기까지 구현이 된다면 클릭 카운트를 1번으로 했을 때 조건에 들어가 카드 내기를 대기하도록
      
                if (_lastClickedTileindex == -1 || _mc.tiles[currentClickedTileindex] == null)
                {

                    if (tileClickCount == 2)
                    {
                        var _piece = Instantiate(this.piece, _mc.tiles[currentClickedTileindex].transform);
                        _piece.GetComponent<Pc>()?.SetPieceOwner(_playerType);
                        return (_piece, -1);
                    }
                    return (null, 0);
                }
         
                // 범위 공격 + 장애물 처리
                _lastClickedTileindex = -1;
                return (null, 1);
           
        };

        SecondTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {           
            currentClickedTileindex = tileNumber;
            if (_mc.tiles[currentClickedTileindex]._piece.GetPieceOwner() == _playerType)
            {
                if (tileClickCount >= 2 && _lastClickedTileindex == currentClickedTileindex )
                {
                    Debug.Log("자신의 말을  골랐습니다");
                    _lastClickedTileindex = -1;
                    return (true, 0);
                }else if (_lastClickedTileindex != -1)
                { // 공격턴에 아군 선택 상황
                    var damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    var attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;

                    if(attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        Debug.Log("아군을 직접적으로 공격할 수 없습니다");
                    }
                    else if (attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        Debug.Log("아군을 직접적으로 공격할 수 없습니다");
                    }
                    else if (attackingPiece._attackType == Pc.AttackType.BUFF)
                    {
                        attackingPiece.Buff(damagedPiece);
                        Debug.Log("아군을 치료했습니다" + damagedPiece.name +"의 Hp:" + damagedPiece.Hp);
                    }
                    _lastClickedTileindex = -1;
                    return (true, 0);
                }
                // 나의 말일 때 조건 충족
                // 공격 대기 메소드 실행 단 아직 구현이 안되있으니
                // 임의의 조건문을 사용해 구현 하겠다

                //공격을 하기 위해서는 다른 말을 선택해야하니 공격자의 인덱스를 저장
                _lastClickedTileindex = currentClickedTileindex;
                Debug.Log("공격할 말을 선택하세요" + _lastClickedTileindex);
            }
            else 
            {
                // 적의 말일 때 조건 충족
                // 말의 정보를 보여줌
                if (_lastClickedTileindex != -1 )
                { // 공격턴에 적 선택 상황
                    var damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    var attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;
                    if (attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        attackingPiece.ChoseAttack(damagedPiece);
                        Debug.Log("적을 공격했습니다" + damagedPiece.name + "의 Hp:" + damagedPiece.Hp);

                    }
                    else if (attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        attackingPiece.RangeAttack(currentClickedTileindex);
                        Debug.Log("적을 공격했습니다" + damagedPiece.name + "의 Hp:" + damagedPiece.Hp);

                    }
                    else if (attackingPiece._attackType == Pc.AttackType.BUFF)
                    {                        
                        Debug.Log("적에게 버프를 줄 수 없습니다");
                    }
                    _lastClickedTileindex = -1;
                }
                else {
                    Debug.Log("적의 말 입니다");
                    return (true, 0);
                }
            }
            return (true, 0);
        };
    }

}
