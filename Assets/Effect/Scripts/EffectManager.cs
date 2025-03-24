using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject effectPrefab;
    public EffectPool effectPool; // 오브젝트 풀링 시스템
    public float effectDuration = 2f;
    public bool usePooling = true; // 풀링 여부 설정

    public void SpawnEffect(Vector3 position)
    {
        if (usePooling && effectPool != null)
        {
            // 풀링 시스템 사용
            GameObject effect = effectPool.GetFromPool(position);
            StartCoroutine(ReturnAfterDelay(effect, effectDuration));
        }
        else
        {
            // 풀링 없이 Instantiate & Destroy 방식 사용
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity);
            Destroy(effect, effectDuration);
        }
    }

    private System.Collections.IEnumerator ReturnAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        effectPool.ReturnToPool(effect);
    }
}