using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using static Piece;
using static RuleManager;

public class AITurnState : MonoBehaviour, IState
{ 

    public StateMachine Fsm { get; set; }
    private GameObject BlockPanel;
    public List<bool> AICosts = new List<bool>();
    public void Enter(Piece.Owner owner)
    {
        // 코스트 최대 카운트 증가
        if (AICosts.Count < 10)
        {
            AICosts.Add(true);
        }
        // 코스트 활성화
        for (int i=0; i<AICosts.Count; i++) {
            AICosts[i] = true;
        }
        Debug.Log("현재 코스트: " + AICosts.Count);
        GameManager.Instance.cp.SetCost(AICosts);

        GameManager.Instance.Costs = AICosts;

        //클릭 막이 패널
        if (BlockPanel == null)
        {
            BlockPanel = Instantiate(GameManager.Instance.BlockPanelPrefab, GameManager.Instance.canvasTransform);
        }
        BlockPanel.SetActive(true);

        //렌즈룰
        GameManager.Instance.ruleManager.UpdateForbiddenMoves(owner);
        //타일 On
        GameManager.Instance.SetTileClickEvent();
        //모든 피스 공격 초기화
        GameManager.Instance.PiecesInit();
        //타이머 on
        GameManager.Instance.gamePanelController.StartTimer();
        int level = GameManager.Instance.PlayerLevel;

        if (level <= 3)
        {
            StartCoroutine(PutPiece(owner));
        }
        else if (level <= 7) {

            StartCoroutine(PutPieceAndOnlyAttack(owner));
        } 
        else if(level <= 12) {
            StartCoroutine(PutPieceThenAttackandHeal(owner));
        }
        else
        {
            StartCoroutine(AttackandHealThenPutPiece(owner));
        }



        //턴 텍스트 설정
        TurnPanelController tp = FindObjectOfType<TurnPanelController>();
        tp.ShowTurnText(owner);
    }

    public void Exit(Piece.Owner owner)
    {
        BlockPanel.SetActive(false);
        // GameManager.Instance.notationManager.PrintAll();
        GameManager.Instance.ruleManager.DeleteForviddensOnMap();
        GameManager.Instance.SetTileClickEventOff();
        GameManager.Instance.SetFalseIsAlreadySetPiece();
        GameManager.Instance.AllTileClickCountSetZero();
        Debug.Log("AITurnState 나갔습니다");
    }

    IEnumerator PutPiece(Piece.Owner owner)
    {
        MessageManager.Instance.ShowMessagePanel("상대의 턴 입니다");
        yield return new WaitForSeconds(1.6f);
        int randomTime = GetRandomValue();
        MessageManager.Instance.ShowMessagePanel("상대가 좋은 위치를 물색하고 있습니다", randomTime);
        // 최적의 인덱스 받아오기
        yield return new WaitForSeconds(randomTime + 0.7f);
        
        (int x, int y) bestMoveIndex = GameManager.Instance.ruleManager.FindOptimalMove(owner);

        int index = bestMoveIndex.y * 8 + bestMoveIndex.x;
        MessageManager.Instance.ShowMessagePanel("상대가 말을 배치합니다");
        Tile selectedTile = GameManager.Instance.Mc.tiles[index];
        //선택된 타일 클릭 발생
        selectedTile.OnClickTileButton();
        List<DeckManager.Card> HandDeck = GameManager.Instance._handManager.GetPlayerAorBHandCards(owner);
        if (HandDeck != null) { 
        
            GameManager.Instance._handManager.OnCardSelected(HandDeck.FirstOrDefault());
            yield return new WaitForSeconds(2);
        }
        GameManager.Instance.OnButtonClickFinishMyTurn();
     }

    IEnumerator PutPieceAndOnlyAttack(Piece.Owner owner)
    {
        MessageManager.Instance.ShowMessagePanel("상대의 턴 입니다");
        yield return new WaitForSeconds(1.6f);
        int randomTime = GetRandomValue();
        MessageManager.Instance.ShowMessagePanel("상대가 좋은 위치를 물색하고 있습니다", randomTime);
        // 최적의 인덱스 받아오기
        yield return new WaitForSeconds(randomTime + 0.7f);

        (int x, int y) bestMoveIndex = GameManager.Instance.ruleManager.FindOptimalMove(owner);

        int index = bestMoveIndex.y * 8 + bestMoveIndex.x;
        MessageManager.Instance.ShowMessagePanel("상대가 말을 배치합니다");
        Tile selectedTile = GameManager.Instance.Mc.tiles[index];
        //선택된 타일 클릭 발생
        selectedTile.OnClickTileButton();
        List<DeckManager.Card> HandDeck = GameManager.Instance._handManager.GetPlayerAorBHandCards(owner);
        if (HandDeck != null)
        {

            GameManager.Instance._handManager.OnCardSelected(HandDeck.FirstOrDefault(card => card.pieceCost == HandDeck.Min(c => c.pieceCost)));
            yield return new WaitForSeconds(2);
        }
   

        //공격 순서 가져오기
        List<int> attackPriority = GameManager.Instance.ruleManager.FindPiecesWithAttackRange(owner);
        string priorityString = "";
        foreach (var priority in attackPriority)
        {
            priorityString += " " + priority + " ";
        }
        Debug.Log("우선순위 : " + priorityString);

        int selectedDamagedTile = GameManager.Instance.ruleManager.FindBestAttackTarget(attackPriority.FirstOrDefault(), owner);

        //공격
        for (int i = 0; i < attackPriority.Count; i++)
        {

            Piece pc = GameManager.Instance.Mc.tiles[attackPriority[i]].Piece;
            if (pc == null) continue;
            if (pc.attackType == AttackType.BUFF)  continue; 

            if (pc.cost <= AICosts.Count(x => x))
            {
                //공격자 선택
                Tile selectedAttackTile = GameManager.Instance.Mc.tiles[attackPriority[i]];
                //선택된 타일 클릭 발생
                selectedAttackTile.OnClickTileButton();
                yield return new WaitForSeconds(1.2f);
                var RANGE = GameManager.Instance.CanAttackRangeCalculate(attackPriority[i], pc.GetAttackRange());
                //이전 공격자가 지금 Piece 사거리 내에 있으면 우선적으로 공격
                if (!RANGE.Contains(selectedDamagedTile)) {
                    selectedDamagedTile = GameManager.Instance.ruleManager.FindBestAttackTarget(attackPriority[i], owner);
                }
                if (selectedDamagedTile < 0) {
                    continue; 
                }
                GameManager.Instance.Mc.tiles[selectedDamagedTile].OnClickTileButton();
                yield return new WaitForSeconds(1.2f);
            }
        }

        GameManager.Instance.OnButtonClickFinishMyTurn();
    }
    IEnumerator PutPieceThenAttackandHeal(Piece.Owner owner)
    {
        MessageManager.Instance.ShowMessagePanel("상대의 턴 입니다");
        yield return new WaitForSeconds(1.6f);
        int randomTime = GetRandomValue();
        MessageManager.Instance.ShowMessagePanel("상대가 좋은 위치를 물색하고 있습니다", randomTime);
        // 최적의 인덱스 받아오기
        yield return new WaitForSeconds(randomTime + 0.7f);

        (int x, int y) bestMoveIndex = GameManager.Instance.ruleManager.FindOptimalMove(owner);

        int index = bestMoveIndex.y * 8 + bestMoveIndex.x;
        MessageManager.Instance.ShowMessagePanel("상대가 말을 배치합니다");
        Tile selectedTile = GameManager.Instance.Mc.tiles[index];
        //선택된 타일 클릭 발생
        selectedTile.OnClickTileButton();
        List<DeckManager.Card> HandDeck = GameManager.Instance._handManager.GetPlayerAorBHandCards(owner);
        if (HandDeck != null)
        {

            GameManager.Instance._handManager.OnCardSelected(HandDeck.FirstOrDefault(card => card.pieceCost == HandDeck.Min(c => c.pieceCost)));
            yield return new WaitForSeconds(2);
        }

        yield return AttackAndHeal(owner);

        GameManager.Instance.OnButtonClickFinishMyTurn();
    }

    IEnumerator AttackandHealThenPutPiece(Piece.Owner owner)
    {
        MessageManager.Instance.ShowMessagePanel("상대의 턴 입니다");
        yield return new WaitForSeconds(1.6f);
        int randomTime = GetRandomValue();
        yield return new WaitForSeconds(1.6f);

        yield return AttackAndHeal(owner);


        MessageManager.Instance.ShowMessagePanel("상대가 좋은 위치를 물색하고 있습니다", randomTime);
        // 최적의 인덱스 받아오기
        yield return new WaitForSeconds(randomTime + 0.7f);

        (int x, int y) bestMoveIndex = GameManager.Instance.ruleManager.FindOptimalMove(owner);

        int index = bestMoveIndex.y * 8 + bestMoveIndex.x;
        MessageManager.Instance.ShowMessagePanel("상대가 말을 배치합니다");
        Tile selectedTile = GameManager.Instance.Mc.tiles[index];
        //선택된 타일 클릭 발생
        selectedTile.OnClickTileButton();
        List<DeckManager.Card> HandDeck = GameManager.Instance._handManager.GetPlayerAorBHandCards(owner);
        if (HandDeck != null)
        {

            GameManager.Instance._handManager.OnCardSelected(HandDeck.FirstOrDefault(card => card.pieceCost == HandDeck.Min(c => c.pieceCost)));
            yield return new WaitForSeconds(2);
        }

        yield return AttackAndHeal(owner);
        GameManager.Instance.OnButtonClickFinishMyTurn();
    }

    IEnumerator AttackAndHeal(Piece.Owner owner) {
        //공격 순서 가져오기
        List<int> attackPriority = GameManager.Instance.ruleManager.FindPiecesWithAttackRange(owner);
        string priorityString = "";
        foreach (var priority in attackPriority)
        {
            priorityString += " " + priority + " ";
        }
        Debug.Log("우선순위 : " + priorityString);


        //공격
        for (int i = 0; i < attackPriority.Count; i++)
        {
            Piece pc = GameManager.Instance.Mc.tiles[attackPriority[i]].Piece;
            if(pc == null)  continue; 
            if (pc.isAlreadyAttack == true) continue; 
            if (pc.cost <= AICosts.Count(x => x))
            {

                int selectedDamagedTile = GameManager.Instance.ruleManager.FindBestAttackTarget(attackPriority[i], owner);


                if (pc.attackType == AttackType.BUFF)
                {
                    //공격자 선택
                    Tile selectedAttackTile = GameManager.Instance.Mc.tiles[attackPriority[i]];
                    //선택된 타일 클릭 발생
                    selectedAttackTile.OnClickTileButton();
                    yield return new WaitForSeconds(1.2f);
                    //회복 받는 타일 선택
                    int selectedHealTile = GameManager.Instance.ruleManager.FindWeakestConsecutiveAllyInRange(attackPriority[i], owner);
                    if (selectedHealTile < 0) {
                        continue; 
                    }
                    GameManager.Instance.Mc.tiles[selectedHealTile].OnClickTileButton();
                    yield return new WaitForSeconds(1.2f);
                }
                else
                {
                    //공격자 선택
                    Tile selectedAttackTile = GameManager.Instance.Mc.tiles[attackPriority[i]];
                    //선택된 타일 클릭 발생
                    selectedAttackTile.OnClickTileButton();
                    yield return new WaitForSeconds(1.2f);
                    //공격 받는 타일 선택

                    var RANGE = GameManager.Instance.CanAttackRangeCalculate(attackPriority[i], pc.GetAttackRange());
                    //이전 공격자가 지금 Piece 사거리 내에 있으면 우선적으로 공격
                    if (!RANGE.Contains(selectedDamagedTile))
                    {
                        selectedDamagedTile = GameManager.Instance.ruleManager.FindBestAttackTarget(attackPriority[i], owner);
                    }
                    if (selectedDamagedTile < 0) {
                        continue; 
                    }
                    GameManager.Instance.Mc.tiles[selectedDamagedTile].OnClickTileButton();
                    yield return new WaitForSeconds(1.2f);
                }
            }
        }
    }

    private int GetRandomValue()
    {
        System.Random random = new System.Random();
        return random.Next(1, 6); // 1 이상 11 미만 (즉, 1~10 사이의 값)
    }
}