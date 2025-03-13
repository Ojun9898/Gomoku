using UnityEngine;
using DG.Tweening;

public class CameraRandomMove : MonoBehaviour
{
    public float moveDuration = 1.5f; // 이동 시간
    public float delayBetweenMoves = 0.5f; // 이동 후 대기 시간

    void Start()
    {
        MoveToRandomPosition();
    }

    void MoveToRandomPosition()
    {
        float randomX = Random.Range(0f, 12f);
        float randomY = Random.Range(-6f, 0f);
        Vector3 targetPosition = new Vector3(randomX, randomY, transform.position.z); // Z값 고정

        // DoTween으로 이동 후 다시 MoveToRandomPosition 호출하여 무한 반복
        transform.DOMove(targetPosition, moveDuration)
            .SetEase(Ease.InOutSine) // 부드러운 움직임
            .OnComplete(() => Invoke(nameof(MoveToRandomPosition), delayBetweenMoves)); // 일정 시간 후 반복
    }
}
