using UnityEngine;
using UnityEngine.EventSystems;

public class HeartsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int originHearts = 3;
    [SerializeField] private GameObject Hearts; // 하트 프리팹
    private int leftHearts;

    void Start()
    {
        leftHearts = originHearts;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < leftHearts; i++)
        {
            Hearts.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        for (int i = 0; i < leftHearts; i++)
        {
            Hearts.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // 공격 받으면 leftHearts 감소
}
