using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public EffectManager effectManager;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바를 누르면 이펙트 생성
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);
            effectManager.SpawnEffect(spawnPosition);
        }
    }
}