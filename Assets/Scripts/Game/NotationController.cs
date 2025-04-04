using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Piece;
using static UnityEngine.Rendering.DebugUI;

public class NotationController : MonoBehaviour
{
    public static int count = 0;
    private Action OnclickButton;
    private int BeforIndex;
    // Start is called before the first frame update
    

    public void DoSomething((int,int) data)
    {
        var pieceOwner = GameManager.Instance.GetCurrentPlayerType();
        int index = data.Item2 * 8 + data.Item1;
        switch (data)
            {
            case var d when d.Item1 <= 63 && d.Item2 <= 63:
                //타일 클릭
                BeforIndex = index;
                Tile selectedTile = GameManager.Instance.Mc.tiles[index];
                //선택된 타일 클릭 발생
                selectedTile.OnClickTileButton();
                break;
            case (66, 64):
                //보병
                Debug.Log("보병");
                SetPiece(PieceType.WARRIOR, pieceOwner, BeforIndex);
                break;
            case (66, 65):
                //궁수
                Debug.Log("궁수");
                SetPiece(PieceType.ARCHER, pieceOwner, BeforIndex);
                break;
            case (66, 66):
                //마법사
                Debug.Log("마법사");
                SetPiece(PieceType.MAGICIAN, pieceOwner, BeforIndex);
                break;
            case (66, 67):
            //기마병
                Debug.Log("기마병");
                SetPiece(PieceType.RANCER, pieceOwner, BeforIndex);
                break;
            case (66, 68):
                //힐러
                Debug.Log("힐러");
                SetPiece(PieceType.HEALER, pieceOwner, BeforIndex);
                break;
            case (64, 65):
                GameManager.Instance.SetCurrentPlayerType(Piece.Owner.PLAYER_B);
                Debug.Log(GameManager.Instance.GetCurrentPlayerType() + "선공");
                break;
            case (65, 64):
                GameManager.Instance.SetCurrentPlayerType(Piece.Owner.PLAYER_A);
                Debug.Log(GameManager.Instance.GetCurrentPlayerType() + "선공");
                break;
            default:
                    Debug.LogWarning($"알 수 없는 데이터: {data}");
                    break;
            }

    }


    public int SettingBuffsAndObstacles() {
        List<(int, int)> values = NotationManager.Instance.currentSelectedFileDatas;
        int ObstacleLastIndex = values.FindLastIndex(v => v == (67, 67));
        int BuffLastIndex = values.FindLastIndex(v => v.Item1 == 68);
        int firstPiece = values.FindIndex(v => v.Item1 == 66);
        int firstPlayerB = values.FindIndex(v => v.Item1 == 64);
        int firstPlayerA = values.FindIndex(v => v.Item1 == 65);

        if (BuffLastIndex <= ObstacleLastIndex)
        {
    
            NotationManager.Instance.NotationElementInit();
            SceneManager.LoadScene("Main");
            return -1;
        }else if(firstPiece <= ObstacleLastIndex)
        {
            
            NotationManager.Instance.NotationElementInit();
            SceneManager.LoadScene("Main");
            return -1;
        }
        else if (firstPlayerB >= ObstacleLastIndex)
        {
        
            NotationManager.Instance.NotationElementInit();
            SceneManager.LoadScene("Main");
            return -1;
        }
        else if (ObstacleLastIndex>=12)
        {
          
            NotationManager.Instance.NotationElementInit();
            SceneManager.LoadScene("Main");
            return -1;
        }
        else if (BuffLastIndex >=22)
        {

            NotationManager.Instance.NotationElementInit();
            SceneManager.LoadScene("Main");
            return -1;
        }


        for (int i = 1; i <= ObstacleLastIndex-1; i += 2)
        {
            SetObstacleAndBuff(values[i], values[i + 1]);
        }
        for (int i = ObstacleLastIndex + 1; i <= BuffLastIndex - 1; i += 2)
        {
            SetObstacleAndBuff(values[i], values[i + 1]);
        }

       return BuffLastIndex+1;
    }


    public void SetObstacleAndBuff((int, int) indexs,(int, int) buffOrObstacle) {
        int index = indexs.Item2 * 8 + indexs.Item1;
        Tile SelectTile = GameManager.Instance.Mc.tiles[index]; 
        switch (buffOrObstacle) {
            case (67, 67):
                var obstacleInstance = Instantiate(GameManager.Instance.Mc.GetObstaclePrefab(), SelectTile.transform);
                SelectTile.SetObstacle(obstacleInstance.GetComponent<Obstacle>());
                break;
            case var d when d.Item1 >= 68:
                var buffInstance = Instantiate(GameManager.Instance.Mc.GetBuffPrefab(), SelectTile.transform);
                SelectTile.buffPrefab = buffInstance;
                switch (buffOrObstacle)
                {
                    case var c when c.Item2 == 67:
                        SelectTile.SetBuff(new AdditionalAttackPowerBuff());
                        break;
                    case var c when c.Item2 == 68:
                        SelectTile.SetBuff(new AdditionalRangeBuff());
                        break;
                    case var c when c.Item2 == 69:
                        SelectTile.SetBuff(new AdditionalHpBuff());
                        break;
                }
                break;
        }
    }

    private void SetPiece(PieceType pt,Piece.Owner owner,int TileIndex) {
        // GameManager의 SetPieceAtTile 메소드를 사용하여 선택된 타일에 말을 생성합니다.
        GameManager.Instance._deckManager.PlayCard(pt, owner);
        GameObject pieceInstance = GameManager.Instance.SetPieceAtTile(TileIndex);
        GameManager.Instance._handManager.isAlreadySetPiece = true;
        Piece pieceComponent = pieceInstance.GetComponent<Piece>();
        Tile _selectedTile = GameManager.Instance.Mc.tiles[TileIndex];
        _selectedTile.Piece = pieceComponent;
        if (_selectedTile.GetBuff() != null)
        {
            _selectedTile.GetBuff().On(pieceComponent);
            Destroy(_selectedTile.buffPrefab);
            _selectedTile.buffPrefab = null;
            _selectedTile.Buff = null;
        }
        // 선택된 타일 초기화
        GameManager.Instance.Mc.tiles[TileIndex].ResetClick();
    }
   
    public void OnClickNextButton() {
        OnclickButton?.Invoke();
        OnclickButton = null;
    }

    public void WatchPlay() { 
        StartCoroutine(PlayDatas());
    }
    IEnumerator PlayDatas() {
        List<(int, int)> values = NotationManager.Instance.currentSelectedFileDatas;
        List<int> endPoint = NotationManager.Instance.EndIndex;
        if (count < endPoint.Count - 1)
        {
            for (int j = endPoint[count]; j < endPoint[count + 1]; j++)
            {
                DoSomething(values[j]);
                yield return new WaitForSeconds(1.5f);
            }
            MessageManager.Instance.ShowMessagePanel("다음 버튼을 눌러주세요");
        }
        else { 
            MessageManager.Instance.ShowMessagePanel("모든 수를 봤습니다");
            yield return new WaitForSeconds(1.6f);
            MessageManager.Instance.ShowMessagePanel("오른쪽 위 버튼을 통해 나갈 수 있습니다");
            yield return new WaitForSeconds(1.6f);
        }
        yield return null;
        OnclickButton = () => {
            count++;
            var changeowner = GameManager.Instance.GetCurrentPlayerType() == Piece.Owner.PLAYER_A ? Piece.Owner.PLAYER_B : Piece.Owner.PLAYER_A;
            GameManager.Instance.SetCurrentPlayerType(changeowner);
            GameManager.Instance.GetFSM().ChangeState<NotationState>(changeowner);
        };
       
    }

    public void SetCountZero() {
        NotationManager.Instance.NotationElementInit();
        count = 0;
        BeforIndex = 0;
    }
}
