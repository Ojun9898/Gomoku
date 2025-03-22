using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CardsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject CardPanelPrefab;

    private GameObject cardPanel;
    private RectTransform cardPanelRect;
    private bool isPointerInside = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerInside = true;
        ShowCardPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        CloseCardPanel();
    }

    private void ShowCardPanel()
    {
        if (cardPanel == null)
        {
            cardPanel = Instantiate(CardPanelPrefab, this.transform);
            var cardPanelScript = cardPanel.GetComponent<CardPanel>();

            // CardPanel의 이벤트를 구독해서 CardsController에서 처리
            if (cardPanelScript != null)
            {
                cardPanelScript.OnEnter += () => isPointerInside = true;
                cardPanelScript.OnExit += () => 
                {
                    isPointerInside = false;
                    CloseCardPanel();
                };
            }
        }

        cardPanelRect = cardPanel.GetComponent<RectTransform>();
        cardPanelRect.anchoredPosition = new Vector2(-500f, 380f);
        cardPanel.SetActive(true);
        cardPanelRect.DOLocalMoveX(9f, 0.6f);
    }

    private void CloseCardPanel()
    {
        if (!isPointerInside && cardPanel != null)
        {
            cardPanelRect.DOLocalMoveX(-500f, 0.3f).OnComplete(() => cardPanel.SetActive(false));
        }
    }
}
