using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CardUI;

[Serializable]
public class NotationIndex
{
    public int x;
    public int y;

    public NotationIndex(int _x, int _y)
    {
        this.x = _x;
        this.y = _y;
    }
}

[Serializable]
public class IntPairList
{
    public List<NotationIndex> pairs = new List<NotationIndex>();
}

public class NotationManager : Singleton<NotationManager>
{
    private string directoryPath;

    private List<(int, int)> values = new List<(int, int)>();

    //전체 파일 저장 리스트
    public List<string> recentFiles = new List<string>();
    //현재 선택된 파일 정보
    public List<(int,int)> currentSelectedFileDatas = new List<(int, int)>();
    public List<(string,string)> recentFileNames = new List<(string, string)>();
    public List<int> EndIndex = new List<int>();
    public int startPoint;

    private void Start()
    {
        NotationManagerinit();
    }

    public void NotationManagerinit()
    {
        directoryPath = Path.Combine(Application.persistentDataPath, "NotationData");

        // 📌 NotationData 폴더가 없으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log($"폴더 생성 완료: {directoryPath}");
        }
    }

    public List<(int, int)> GetValues()
    {
        return values;
    }

    public void PrintAll()
    {
        Debug.Log(string.Join(", ", values));
    }

    public void AddClickIndex(int x, int y)
    {
        values.Add((x, y));
    }

    public void AddFinishTurn()
    {
        values.Add((64, 64));
    }
    public void AddObstacleorBuff(List<(int,int)> obstacleList) {
        values.AddRange(obstacleList);
    }
    public void AddHowsFirst(Piece.Owner owner)
    {
        switch (owner)
        {
            case Piece.Owner.PLAYER_A:
                values.Add((65, 64));
                break;
            case Piece.Owner.PLAYER_B:
                values.Add((64, 65));
                break;
        }
    }

    public void AddPieceType(Piece.PieceType pieceType)
    {
        switch (pieceType)
        {
            case Piece.PieceType.WARRIOR:
                values.Add((66, 64));
                break;
            case Piece.PieceType.MAGICIAN:
                values.Add((66, 66));
                break;
            case Piece.PieceType.ARCHER:
                values.Add((66, 65));
                break;
            case Piece.PieceType.HEALER:
                values.Add((66, 68));
                break;
            case Piece.PieceType.RANCER:
                values.Add((66, 67));
                break;
            default:
                break;
        }
    }

    // 📌 1. 데이터 저장 (NotationData 폴더 내에 저장)
    public void SaveData(string playerName)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{playerName}_{timestamp}.json";
        string filePath = Path.Combine(directoryPath, fileName);

        IntPairList wrapper = new IntPairList();
        foreach (var (first, second) in values)
        {
            wrapper.pairs.Add(new NotationIndex(first, second));
        }

        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(filePath, json);
        Debug.Log($"데이터 저장 완료: {filePath}");

        // 📌 최근 파일 유지 및 가장 오래된 파일 삭제
        MaintainRecentFiles();
    }

      // 📌 최근 10개 파일 유지하고 가장 오래된 파일 삭제
    private void MaintainRecentFiles()
    {
        var files = Directory.GetFiles(directoryPath, "*.json")
            .OrderByDescending(File.GetCreationTime)
            .ToList();

        if (files.Count > 10)
        {
            string oldestFile = files.Last(); // 가장 오래된 파일 (목록의 마지막 요소)
            File.Delete(oldestFile);
            Debug.Log($"가장 오래된 파일 삭제: {oldestFile}");
        }
    }



    // 📌 2. 최근 10개 파일 찾기 (NotationData 폴더에서 검색)
    public void GetRecentFiles()
    {
        if (!Directory.Exists(directoryPath))
        {
            Debug.LogWarning("저장 폴더 없음.");
            return;
        }

        // 파일 정렬 (가장 최근 저장된 파일 순)
        var files = Directory.GetFiles(directoryPath, "*.json")
            .OrderByDescending(File.GetCreationTime)
            .Take(10)
            .ToList();

        recentFiles = files;
        Debug.Log("최근 파일 10개 찾기 완료.");
    }

    // 📌 3. 특정 파일에서 데이터 불러오기 (NotationData 폴더 내에서 검색)
    public void LoadDataFromFile(int index)
    {
        if (!File.Exists(recentFiles[index]))
        {
            Debug.LogWarning($"파일 없음: {recentFiles[index]}");
        }

        string json = File.ReadAllText(recentFiles[index]);
        Debug.Log($"읽은 JSON: {json}");

        if (string.IsNullOrWhiteSpace(json))
        {
            Debug.LogError("파일이 비어 있음.");
        }

        IntPairList wrapper = JsonUtility.FromJson<IntPairList>(json);

        if (wrapper == null || wrapper.pairs == null)
        {
            Debug.LogError("JSON 파싱 실패 또는 데이터 손상.");
        }

        List<(int, int)> data = wrapper.pairs.Select(p => (p.x, p.y)).ToList();
        Debug.Log($"데이터 불러오기 완료: {recentFiles[index]}");
        currentSelectedFileDatas = data;
    }
    // 📌 파일명에서 플레이어 이름과 날짜 분리
    public (string playerName, string timestamp) ParseFileName(string fileName)
    {
        if (string.IsNullOrEmpty(fileName) || !fileName.EndsWith(".json"))
        {
            Debug.LogWarning("올바르지 않은 파일명: " + fileName);
            return ("", "");
        }

        fileName = Path.GetFileNameWithoutExtension(fileName); // 확장자 제거
        int firstUnderscore = fileName.IndexOf('_');
        if (firstUnderscore == -1)
        {
            Debug.LogWarning("파일명 형식이 올바르지 않습니다: " + fileName);
            return ("", "");
        }

        string playerName = fileName.Substring(0, firstUnderscore);
        string timestamp = fileName.Substring(firstUnderscore + 1);
        return (playerName, timestamp);
    }
    //파일명 얻기
    public void PrintRecentFileNames()
    {
        recentFiles.Clear();
        recentFileNames.Clear();
        GetRecentFiles();
        foreach (var file in recentFiles)
        {
            string fileName = Path.GetFileName(file);
            var (playerName, timestamp) = ParseFileName(fileName);
            recentFileNames.Add((playerName, timestamp));
        }
    }

    /// <summary>
    /// 찾으려는 값 인덱스 반환
    /// </summary>
    /// <param name="data"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public void GetIndexesOf(List<(int, int)> data)
    {
        EndIndex.Clear();
        List<int> indexes = new List<int>();

        for (int i = 0; i < data.Count; i++)
        {
            if (data[i] == (64,64))
            {
                indexes.Add(i);
            }
        }
    
        EndIndex = indexes;
        EndIndex.Insert(0, startPoint);
     
    }
    
    public void NotationElementInit() {
        values.Clear();
        recentFiles.Clear();
        currentSelectedFileDatas.Clear();
        recentFileNames.Clear();
        EndIndex.Clear();
    }

    protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
    }
}