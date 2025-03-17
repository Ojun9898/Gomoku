using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public enum TileType
{
    Obstacle, Buff, PlayerA, PlayerB
}

public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesParent;
    private Tile[] _tiles;
    private TileType[] _currentTileInfo;

    private void Awake()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        // Mc 스크립트 가져오기
        Mc mc = tilesParent.GetComponent<Mc>();

        // 타일 생성 및 Mc에 등록
        for (int i = 0; i <= 7; i++)
        {
            for (int j = 0; j <= 7; j++)
            {
                Vector2 tilePos = new Vector2(j, i);
                var tileInstance = Instantiate(tilePrefab, tilePos, Quaternion.identity, tilesParent);
                

                // 생성된 타일을 Mc에 등록
                Tile tileComponent = tileInstance.GetComponent<Tile>();
                tileComponent.tileNumber = i * 8 + j;
                mc.tiles.Add(tileComponent);
            }
        }
    }


    private void ActiveObstacle()
    {
        // TODO: 랜덤 인덱스에 장애물 활성화 구현
    }

    private void ActiveBuff()
    {
        // TODO: 랜덤 인덱스에 버프 활성화 구현
    }

}
