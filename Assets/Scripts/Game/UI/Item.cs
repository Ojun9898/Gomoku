using UnityEngine;

public class Item : MonoBehaviour
{
    private GamePanelController gamePanelController;

    void Start()
    {
        GameObject gamePanelObject = GameObject.Find("GamePanel");

        if (gamePanelObject != null)
            gamePanelController = gamePanelObject.GetComponent<GamePanelController>();

    }

    public void ItemTimer() // 타이머 초기화 아이템
    {
        if (gamePanelController != null)
        {
            MessageManager.Instance.ShowMessagePanel("아이템 사용! 타이머를 초기화 합니다");
            gamePanelController.StopTimer();
            gamePanelController.StartTimer();
        }
    }
}
