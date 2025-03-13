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


    // ��Ÿ�� ���߿� ��� ��ȭ������ �ϴ��� ������ ����
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
            // Ŭ�� ī��Ʈ 2������ ������ �ξ��µ�
            // ī�� ������� ������ �ȴٸ� Ŭ�� ī��Ʈ�� 1������ ���� �� ���ǿ� �� ī�� ���⸦ ����ϵ���
      
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
         
                // ���� ���� + ��ֹ� ó��
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
                    Debug.Log("�ڽ��� ����  ������ϴ�");
                    _lastClickedTileindex = -1;
                    return (true, 0);
                }else if (_lastClickedTileindex != -1)
                { // �����Ͽ� �Ʊ� ���� ��Ȳ
                    var damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    var attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;

                    if(attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        Debug.Log("�Ʊ��� ���������� ������ �� �����ϴ�");
                    }
                    else if (attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        Debug.Log("�Ʊ��� ���������� ������ �� �����ϴ�");
                    }
                    else if (attackingPiece._attackType == Pc.AttackType.BUFF)
                    {
                        attackingPiece.Buff(damagedPiece);
                        Debug.Log("�Ʊ��� ġ���߽��ϴ�" + damagedPiece.name +"�� Hp:" + damagedPiece.Hp);
                    }
                    _lastClickedTileindex = -1;
                    return (true, 0);
                }
                // ���� ���� �� ���� ����
                // ���� ��� �޼ҵ� ���� �� ���� ������ �ȵ�������
                // ������ ���ǹ��� ����� ���� �ϰڴ�

                //������ �ϱ� ���ؼ��� �ٸ� ���� �����ؾ��ϴ� �������� �ε����� ����
                _lastClickedTileindex = currentClickedTileindex;
                Debug.Log("������ ���� �����ϼ���" + _lastClickedTileindex);
            }
            else 
            {
                // ���� ���� �� ���� ����
                // ���� ������ ������
                if (_lastClickedTileindex != -1 )
                { // �����Ͽ� �� ���� ��Ȳ
                    var damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    var attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;
                    if (attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        attackingPiece.ChoseAttack(damagedPiece);
                        Debug.Log("���� �����߽��ϴ�" + damagedPiece.name + "�� Hp:" + damagedPiece.Hp);

                    }
                    else if (attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        attackingPiece.RangeAttack(currentClickedTileindex);
                        Debug.Log("���� �����߽��ϴ�" + damagedPiece.name + "�� Hp:" + damagedPiece.Hp);

                    }
                    else if (attackingPiece._attackType == Pc.AttackType.BUFF)
                    {                        
                        Debug.Log("������ ������ �� �� �����ϴ�");
                    }
                    _lastClickedTileindex = -1;
                }
                else {
                    Debug.Log("���� �� �Դϴ�");
                    return (true, 0);
                }
            }
            return (true, 0);
        };
    }

}
