using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;



public class MapController : MonoBehaviour
{
    [SerializeField] private GameObject obstacle;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Transform tilesParent;
    private Tile[] _tiles;
    private int width = 8;
    public List<Tile> tiles;

    private void Awake()
    {
        CreateMap();
    }

    public void CreateMap()
    {
        for (int i = 0; i <= width -1; i++)
        {
            for (int j = 0; j <= width - 1; j++)
            {
                Vector2 tilePos = new Vector2(j, i);
                tilePos = new Vector2(tilePos.x -3.5f, tilePos.y -3.5f);
                var tileInstance = Instantiate(tilePrefab, tilePos, Quaternion.identity, tilesParent);
               
                Tile tileComponent = tileInstance.GetComponent<Tile>();
                tileComponent.tileNumber = i * 8 + j;
                tiles.Add(tileComponent);
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
