using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemPanelController : Item, IPointerEnterHandler, IPointerExitHandler
{
   [SerializeField] private Button[] itemButtons;
   public Action OnEnter;
   public Action OnExit;

   private List<string> availableItems = new List<string>();
   private Dictionary<Button, UnityAction> buttonListeners = new Dictionary<Button, UnityAction>(); // 버튼과 리스너를 매핑

   // 데이터 로드를 담당하는 메서드
   public void LoadInventoryData()
   {
      availableItems = BuyManager.Instance.GetAvailableItemInfo();

      string itemsStr = string.Join(", ", availableItems);
      Debug.Log("Available items: " + itemsStr);
   }

   // UI 업데이트를 담당하는 메서드
   // UI 업데이트를 담당하는 메서드
   public void UpdateInventoryUI()
   {
      if (availableItems == null || availableItems.Count == 0 || availableItems.All(item => string.IsNullOrWhiteSpace(item)))
      {
         Debug.Log("인벤토리에 아이템이 없습니다.");

         foreach (var button in itemButtons)
         {
            button.gameObject.SetActive(false);
         }
         return;
      }

      // 중복 제거된 아이템 목록 생성
      HashSet<string> uniqueItems = new HashSet<string>(availableItems);

      // UI 버튼 리스트를 가져와서 순서대로 아이템 이름 적용
      itemButtons = GetComponentsInChildren<Button>();

      int i = 0;

      foreach (string item in uniqueItems)
      {
         // 기존 리스너가 있으면 제거
         if (buttonListeners.ContainsKey(itemButtons[i]))
         {
            itemButtons[i].onClick.RemoveListener(buttonListeners[itemButtons[i]]);
         }

         // 버튼에 새로운 텍스트 설정
         itemButtons[i].GetComponentInChildren<TMP_Text>().text = item;

         // 각 버튼에 클릭 이벤트를 추가하여 useItem을 설정
         int index = i;
         UnityAction newListener = () => OnClickItemButton(uniqueItems.ElementAt(index));

         // 리스너가 없을 때만 추가
         itemButtons[i].onClick.AddListener(newListener);
         buttonListeners[itemButtons[i]] = newListener;

         itemButtons[i].gameObject.SetActive(true);
         i++;
      }

      // 남은 버튼은 비활성화
      for (; i < itemButtons.Length; i++)
      {
         itemButtons[i].gameObject.SetActive(false);
      }
   }

   public void RefreshInventory()
   {
      LoadInventoryData();  // 데이터를 불러옴
      UpdateInventoryUI();  // UI를 업데이트
   }


   public void OnClickItemButton(string selectedItem)
   {
      // 클릭된 아이템을 useItem으로 설정
      BuyManager.Instance.UpdateUseItems(selectedItem);

      //아이템 실행
      ItemTimer();

      //인벤토리 새로고침
      RefreshInventory();
   }

   // ShowItemPanel 유지
   public void OnPointerEnter(PointerEventData eventData)
   {
      OnEnter?.Invoke();  // ItemsController로 이벤트 전달
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      OnExit?.Invoke();   // ItemsController로 이벤트 전달
   }
}
