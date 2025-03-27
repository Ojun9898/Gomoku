using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;
using System.Diagnostics;

public class CardPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
   [SerializeField] private Sprite w_archer;
   [SerializeField] private Sprite w_worrior;
   [SerializeField] private Sprite w_magician;
   [SerializeField] private Sprite w_rancer;
   [SerializeField] private Sprite w_healer;
   [SerializeField] private Sprite b_archer;
   [SerializeField] private Sprite b_worrior;
   [SerializeField] private Sprite b_magician;
   [SerializeField] private Sprite b_rancer;
   [SerializeField] private Sprite b_healer;
   [SerializeField] private int cardCount;

    public Action OnEnter;  // CardsController에서 처리할 이벤트
    public Action OnExit;

   void Start()
   {
      SetCardImage();
   }

   // ShowCardPanel 유지
    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter?.Invoke();  // CardsController로 이벤트 전달
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit?.Invoke();   // CardsController로 이벤트 전달
    }

   // 손패 정보(종류-개수)를 받아옴
   public void SetCardInfo()
   {
      
      // 손패 정보: String 나열 -> 배열 변환 -> 이미지 설정
      // 손패 정보: 배열 -> ???
   }

   // 이미지 생성 후 sprite 설정
   public void SetCardImage()
   {
      SetCardInfo();

      // 자식개체로 이미지 15개 생성
      GameObject[] cardImage = new GameObject[cardCount];
      
      for (int i = 0; i < cardCount; i++)
      {
         cardImage[i] = new GameObject("CardImage" + i);
         cardImage[i].transform.SetParent(this.transform);
         cardImage[i].AddComponent<Image>();
      }

      ChangeCardImage();

   }

   public void ChangeCardImage()
   {
   }

}
