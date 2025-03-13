using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Pc;

public class GameManager : Singleton<GameManager>
{    
    [SerializeField] private Mc _mc;
    [SerializeField] private GameObject piece;

    public Func<int, int, (GameObject piece, int caseValue)> FirstTimeTileClickEvent;

    // 런타임 도중에 계속 변화하지만 일단은 지정해 뒀음
    private Owner _playerType = Owner.PLAYER_A;
    private int _currentClickedTileindex;
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
            if (_mc.tiles[currentClickedTileindex]._piece == null && tileClickCount == 2)
            {
                var _piece = Instantiate(this.piece, _mc.tiles[currentClickedTileindex].transform);
                _piece.GetComponent<Pc>()?.SetPieceOwner(_playerType);
                return (_piece, -1);
            }
            return (null, 0);
        };
    }

}
