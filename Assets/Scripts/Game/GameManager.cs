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
    public Func<int, int, (bool isNeedJustOneClick,int caseValue)> SecondTimeTileClickEvent;
    public Action RangeAttackVisualizeEvent;
    public Action RangeAttackResetVisualizeEvent;


    // ��Ÿ�� ���߿� ��� ��ȭ������ �ϴ��� ������ ����
    private Pc.Owner _playerType = Pc.Owner.PLAYER_A;
    private int _currentClickedTileindex;
    private int _lastClickedTileindex = -1;
    private Pc _damagedPiece;
    private Pc _attackingPiece;
    private List<int> _currentPieceCanAttackRange;

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

            // ���� ����

            // + ��ֹ� ó��
            FinishiedAttack();
            return (null, 1);
        };

        SecondTimeTileClickEvent = (tileNumber, tileClickCount) =>
        {           
            currentClickedTileindex = tileNumber;

           /* // ���� ����Piece ���� ���� �����ֱ� Todo: ���� ����
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
                _currentPieceCanAttackRange = CanAttackRangeCalculate(currentClickedTileindex, _mc.tiles[currentClickedTileindex]._piece.GetAttackRange());
                VisualizeAttackRange(_currentPieceCanAttackRange);

                if (tileClickCount >= 2 && _lastClickedTileindex == currentClickedTileindex )
                {
                    Debug.Log("�ڽ��� ����  ������ϴ�");
                    _lastClickedTileindex = -1;
                    return (true, 0);
                }else if (_lastClickedTileindex != -1)
                { // �����Ͽ� �Ʊ� ���� ��Ȳ
                    _damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    _attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;

                    if(_attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        Debug.Log("�Ʊ��� ���������� ������ �� �����ϴ�");
                    }
                    else if (_attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        Debug.Log("�Ʊ��� ���������� ������ �� �����ϴ�");
                    }
                    else if (_attackingPiece._attackType == Pc.AttackType.BUFF)
                    {
                        _attackingPiece.Buff(_damagedPiece);
                        Debug.Log("�Ʊ��� ġ���߽��ϴ�" + _damagedPiece.name +"�� Hp:" + _damagedPiece.Hp);
                    }
                    FinishiedAttack();
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
                    _damagedPiece = _mc.tiles[currentClickedTileindex]._piece;
                    _attackingPiece = _mc.tiles[_lastClickedTileindex]._piece;
                    if (_attackingPiece._attackType == Pc.AttackType.CHOOSE_ATTACK)
                    {
                        _attackingPiece.ChoseAttack(_damagedPiece);
                        Debug.Log("���� �����߽��ϴ�" + _damagedPiece.name + "�� Hp:" + _damagedPiece.Hp);

                    }
                    else if (_attackingPiece._attackType == Pc.AttackType.RANGE_ATTACK)
                    {
                        //attackingPiece.RangeAttack(currentClickedTileindex);
                        Debug.Log("���� �����߽��ϴ�" + _damagedPiece.name + "�� Hp:" + _damagedPiece.Hp);

                    }
                    else if (_attackingPiece._attackType == Pc.AttackType.BUFF)
                    {                        
                        Debug.Log("������ ������ �� �� �����ϴ�");
                    }
           
                    FinishiedAttack();
                }
                else {
                    Debug.Log("���� �� �Դϴ�");
                    return (true, 0);
                }
            }
            return (true, 0);
        };
    }

    /// <summary>
    /// ���� ��Ȳ�� ������ ���� �����ϰ� ��� ��Ȳ�� �ʱ�ȭ�ϴ� �޼ҵ�
    /// </summary>
    private void FinishiedAttack() {
        _damagedPiece = null;
        _attackingPiece = null;
        RangeAttackVisualizeEvent = null;
        _lastClickedTileindex = -1;
        if (_currentPieceCanAttackRange != null) { 
            ResetVisualizeAttackRange(ref _currentPieceCanAttackRange);
        }
    }

    /// <summary>
    /// Piece�� ���� ���� ������ ����ϴ� �޼ҵ� 
    /// ���� VisualizeAttackRange, ResetVisualizeAttackRange �� �Բ� MapController�� �̵��ʿ�
    /// </summary>
    /// <param name="index"> ������ Ÿ�� ��ġ</param>
    /// <param name="attackRange">piece�� ��Ÿ�</param>
    /// <returns> piece�� Range�� ���� ���� ���� ����</returns>
    public List<int> CanAttackRangeCalculate(int index, int attackRange)
    {
        int width = 4;
        int height = 4;

        int y = index / 4;
        int x = index % 4;

        List<int> result = new List<int>();

        // �����¿� 1ĭ ���� ������ ������ ��Ҹ� ���
        for (int dy = -attackRange; dy <= attackRange; dy++)
        {
            for (int dx = -attackRange; dx <= attackRange; dx++)
            {
                int targetX = x + dx;
                int targetY = y + dy;

                // �迭�� ���� ���� �ִ��� üũ
                if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                {
                    // �ڽ��� �����Ϸ��� (x, y) ��ǥ�� �ǳʶٱ�
                    if (targetX == x && targetY == y)
                    {
                        continue; // �ڽ��� ����
                    }

                    // 1D �迭�� 2D ��ġ ����
                    int indexs = targetY * width + targetX;
                    result.Add(indexs);
                }
            }
        }
        return result;
    }
    /// <summary>
    /// piece�� ���� ���� ������ Ÿ�Ͽ� �ð�ȭ �մϴ�
    /// </summary>
    /// <param name="attackRange">CanAttackRangeCalculate ���� ��ȯ �� ��</param>
    private void VisualizeAttackRange(List<int> attackRange)
    {        
        foreach (var index in attackRange)
        {
            _mc.tiles[index].GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
    /// <summary>
    /// ���� ���� �ð�ȭ�� �ʱ�ȭ �մϴ� + _currentPieceCanAttackRange �ʱ�ȭ
    /// </summary>
    /// <param name="attackRange">CanAttackRangeCalculate ���� ��ȯ �� ��</param>
    private void ResetVisualizeAttackRange(ref List<int> attackRange)
    {
        foreach (var index in attackRange)
        {
            _mc.tiles[index].GetComponent<SpriteRenderer>().color = Color.white;
        }
        attackRange = null;
    }
}
