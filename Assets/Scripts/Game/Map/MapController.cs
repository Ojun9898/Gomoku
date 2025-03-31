using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject buffPrefab;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesParent;
    private int width = 8;
    public List<Tile> tiles;
    
    private int _buffIndex;
    private int _obstacleIndex;
    private const int BuffMaxCount = 5;
    private const int ObstacleMaxCount = 5;
    private string sName;
    private readonly HashSet<int> _usedIndexes = new HashSet<int>();
    private List<(int,int)> _obstacleIndexes = new List<(int, int)>();
    private List<(int, int)> _buffIndexes = new List<(int, int)>();

    public void CreateMap()
    {
        for (int i = 0; i <= width -1; i++)
        {
            for (int j = 0; j <= width - 1; j++)
            {
                Vector2 tilePos = new Vector2(j, -i);
                tilePos = new Vector2(tilePos.x -3.5f, tilePos.y +3.5f);
                var tileInstance = Instantiate(tilePrefab, tilePos, Quaternion.identity, tilesParent);
               
                Tile tileComponent = tileInstance.GetComponent<Tile>();
                tileComponent.TileInit();
                tileComponent.tileNumber = i * 8 + j;
                tiles.Add(tileComponent);
            }
        }
        sName = SceneManager.GetActiveScene().name;
       
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && sName == "Game")
        {
            // UI 요소 위를 클릭한 경우는 제외하고
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                // 마우스 위치에서 Raycast를 실행하여 Tile 컴포넌트가 있는지 확인
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (!Physics.Raycast(ray, out hit))
                {
                    GameManager.Instance.FadeCardAndResetClick();
                    GameManager.Instance.FinishedAttack();
                }
                else
                {
                    // 만약 Raycast된 오브젝트에 Tile 컴포넌트가 없다면 카드 패널 비활성화
                    if (hit.collider.GetComponent<Tile>() == null)
                    {
                        GameManager.Instance.FadeCardAndResetClick();
                        GameManager.Instance.FinishedAttack();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tile의 랜덤 인덱스에 장애물 생성
    /// 개수는 1~5개 사이로 랜덤하게 생성
    /// </summary>
    public List<(int,int)> ActiveObstacleInGameScene()
    {
        _usedIndexes.Clear();
        _obstacleIndexes.Clear(); // 중복 방지를 위해 장애물과 버프를 배치하기 전에 초기화

        int obstacleCount = Random.Range(1, ObstacleMaxCount);

        for (int i = 0; i < obstacleCount; i++) // <= 대신 < 사용 (정확한 개수 생성)
        {
            int obstacleIndex;
            int x;
            int y;

            do
            {
                obstacleIndex = Random.Range(0, tiles.Count); // 중복되지 않는 새로운 인덱스 찾기
                 x = obstacleIndex % 8;
                 y = obstacleIndex / 8;
            } while (_usedIndexes.Contains(obstacleIndex)); // 이미 사용된 인덱스라면 다시 뽑기
            _usedIndexes.Add(obstacleIndex);
            _obstacleIndexes.Add((x,y)); // 사용된 인덱스 저장
            _obstacleIndexes.Add((67, 67));
            var obstacleInstance = Instantiate(obstaclePrefab, tiles[obstacleIndex].transform);
            tiles[obstacleIndex].SetObstacle(obstacleInstance.GetComponent<Obstacle>());
        }
        return _obstacleIndexes;
    }

    /// <summary>
    /// Tile의 랜덤 인덱스에 버프 생성
    /// 효과는 3가지 중 랜덤으로 부여됨
    /// 개수는 1~5개 중으로 랜덤하게 생성
    /// </summary>
    public List<(int,int)> ActiveBuffInGameScene()
    {
        int buffCount = Random.Range(1, BuffMaxCount);
        _buffIndexes.Clear();

        for (int i = 0; i < buffCount; i++) // <= 대신 < 사용
        {
            int buffIndex;
            int x;
            int y;
            do
            {
                buffIndex = Random.Range(0, tiles.Count);
                x = buffIndex % 8;
                y = buffIndex / 8;
            } while (_usedIndexes.Contains(buffIndex)); // 중복되지 않는 인덱스 찾기
            _usedIndexes.Add(buffIndex);
            _buffIndexes.Add((x,y));
            var buffInstance = Instantiate(buffPrefab, tiles[buffIndex].transform);
            tiles[buffIndex].buffPrefab = buffInstance;

            // 3가지 효과 중 랜덤 적용
            int functionIndex = Random.Range(0, 3); // 0~2 (원래 코드에서 2번이 실행 안 될 수도 있었음)

            switch (functionIndex)
            {
                case 0:
                    tiles[buffIndex].SetBuff(new AdditionalAttackPowerBuff());
                    _buffIndexes.Add((68,67));
                    break;
                case 1:
                    tiles[buffIndex].SetBuff(new AdditionalRangeBuff());
                    _buffIndexes.Add((68,68));
                    break;
                case 2:
                    tiles[buffIndex].SetBuff(new AdditionalHpBuff());
                    _buffIndexes.Add((68,69));
                    break;
            }
        }
        return _buffIndexes;
    }

    public GameObject GetBuffPrefab() { 
        return buffPrefab;
    }

    public GameObject GetObstaclePrefab()
    {
        return obstaclePrefab;
    }
}
