using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    private readonly HashSet<int> _usedIndexes = new HashSet<int>();

    public void CreateMap()
    {
        for (int i = 0; i <= width -1; i++)
        {
            for (int j = 0; j <= width - 1; j++)
            {
                Vector2 tilePos = new Vector2(j * 1.25f, -i * 1.25f);
                tilePos = new Vector2(tilePos.x -4.375f, tilePos.y +4.375f);
                var tileInstance = Instantiate(tilePrefab, tilePos, Quaternion.identity, tilesParent);
               
                Tile tileComponent = tileInstance.GetComponent<Tile>();
                tileComponent.tileNumber = i * 8 + j;
                tiles.Add(tileComponent);
            }
        }
    }


    /// <summary>
    /// Tile의 랜덤 인덱스에 장애물 생성
    /// 개수는 1~5개 사이로 랜덤하게 생성
    /// </summary>
    private void ActiveObstacle()
    {
        _usedIndexes.Clear(); // 중복 방지를 위해 장애물과 버프를 배치하기 전에 초기화

        int obstacleCount = Random.Range(1, ObstacleMaxCount);

        for (int i = 0; i < obstacleCount; i++) // <= 대신 < 사용 (정확한 개수 생성)
        {
            int obstacleIndex;
            do
            {
                obstacleIndex = Random.Range(0, tiles.Count); // 중복되지 않는 새로운 인덱스 찾기
            } while (_usedIndexes.Contains(obstacleIndex)); // 이미 사용된 인덱스라면 다시 뽑기

            _usedIndexes.Add(obstacleIndex); // 사용된 인덱스 저장
            var obstacleInstance = Instantiate(obstaclePrefab, tiles[obstacleIndex].transform);
            tiles[obstacleIndex].SetObstacle(obstacleInstance.GetComponent<Obstacle>());
        }
    }

    /// <summary>
    /// Tile의 랜덤 인덱스에 버프 생성
    /// 효과는 3가지 중 랜덤으로 부여됨
    /// 개수는 1~5개 중으로 랜덤하게 생성
    /// </summary>
    private void ActiveBuff()
    {
        int buffCount = Random.Range(1, BuffMaxCount);

        for (int i = 0; i < buffCount; i++) // <= 대신 < 사용
        {
            int buffIndex;
            do
            {
                buffIndex = Random.Range(0, tiles.Count);
            } while (_usedIndexes.Contains(buffIndex)); // 중복되지 않는 인덱스 찾기

            _usedIndexes.Add(buffIndex);
            Instantiate(buffPrefab, tiles[buffIndex].transform);

            // 3가지 효과 중 랜덤 적용
            int functionIndex = Random.Range(0, 3); // 0~2 (원래 코드에서 2번이 실행 안 될 수도 있었음)

            switch (functionIndex)
            {
                case 0:
                    tiles[buffIndex].SetBuff(new AdditionalAttackPowerBuff());
                    break;
                case 1:
                    tiles[buffIndex].SetBuff(new AdditionalRangeBuff());
                    break;
                case 2:
                    tiles[buffIndex].SetBuff(new AdditionalHpBuff());
                    break;
            }
        }
    }

}
