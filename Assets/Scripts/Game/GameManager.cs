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

    // ��Ÿ�� ���߿� ��� ��ȭ������ �ϴ��� ������ ����
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
    /// ���� �޴��� �ʱ�ȭ
    /// </summary>
    private void InitGameManager()
    {
        // Map���� Ÿ�� ������ �������� �޼ҵ�
        // piece ������ �������� �޼ҵ�
        // ī�� ���� �޼ҵ� 
        SetTileClickEvent();
    }

    //ī�� ���� ��� �޼ҵ�
    //�� ���� ��� �޼ҵ�

    /// <summary>
    /// Ÿ�� Ŭ�� ������ �ο��ϴ� �޼ҵ�
    /// </summary>
    private void SetTileClickEvent()
    {
        FirstTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {
            currentClickedTileindex = tileNumber;
            //ó�� Ŭ�� �� 
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
