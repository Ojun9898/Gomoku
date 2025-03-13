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
    [SerializeField] private GameObject mapPrefab;
    private Tilemap _tilemap;
    private Tile[] _tiles;
    private TileType[] _currentTileInfo;

    public void CreateMap()
    {
        // TODO: 맵 생성 기능 구현
        var mapInstance = Instantiate(mapPrefab);
        _tilemap = mapInstance.GetComponent<Tilemap>();
    }

    private void ActiveObstacle()
    {
        // TODO: 랜덤 인덱스에 장애물 활성화 구현
    }

    private void ActiveBuff()
    {
        // TODO: 랜덤 인덱스에 버프 활성화 구현
    }

    private int GetTileIndex(Vector3 worldPosition)
    {
        if (_tilemap == null)
        {
            Debug.LogWarning("Tilemap이 설정되지 않았습니다!");
        }

        Vector3Int cellPosition = _tilemap.WorldToCell(worldPosition);

        // 타일이 있는지 확인
        if (!_tilemap.HasTile(cellPosition))
        {
            Debug.Log("선택한 위치에 타일이 없습니다!");
        }

        // 8x8 맵 기준으로 인덱스 변환 (0~63)
        int index = cellPosition.y * 8 + cellPosition.x;
        
        Debug.Log($"타일 인덱스: {index} (좌표: {cellPosition})");
        return index;
    }
    
    // TODO: MapController에 사용할지 GameManager에 사용할지 생각해야함.
    // private void Update()
    // {
    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         // 마우스 클릭 위치를 받아와서 변환
    //         Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         worldPosition.z = 0; // 2D 환경이므로 Z 값 0으로 고정
    //         GetTileIndex(worldPosition);
    //     }
    // }
    
}
