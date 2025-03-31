using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class ItemsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject ItemPanelPrefab;

    private GameObject itemPanel;
    private RectTransform ItemPanelRect;
    private bool isPointerInside = false;

    public void OnPointerEnter(PointerEventData eventData)
    {

        isPointerInside = true;
        ShowItemPanel();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerInside = false;
        CloseItemPanel();
    }

    private void ShowItemPanel()
    {
        if (itemPanel == null)
        {
            itemPanel = Instantiate(ItemPanelPrefab, this.transform);
            var itemPanelController = itemPanel.GetComponent<ItemPanelController>();

            if (itemPanelController != null)
            {
                itemPanelController.OnEnter += () => isPointerInside = true;
                itemPanelController.OnExit += () => 
                {
                    isPointerInside = false;
                    CloseItemPanel();
                };

                itemPanelController.RefreshInventory();
            }
        }

        ItemPanelRect = itemPanel.GetComponent<RectTransform>();
        ItemPanelRect.anchoredPosition = new Vector2(-500f, 380f);
        itemPanel.SetActive(true);
        ItemPanelRect.DOLocalMoveX(9f, 0.6f);
    }

    private void CloseItemPanel()
    {
        if (!isPointerInside && itemPanel != null)
        {
            ItemPanelRect.DOLocalMoveX(-500f, 0.3f).OnComplete(() => itemPanel.SetActive(false));
        }
    }
}
