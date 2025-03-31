using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SelectPanelController : MonoBehaviour
{
    public string selectImageName;
    private ProfilePanelController pp;

    void Start()
    {
        pp = FindObjectOfType<ProfilePanelController>();
        
        Button[] buttons = GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => OnClickSelectButton(button.gameObject));
        }   
    }
    public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false));
    }

    public void OnClickSelectButton(GameObject button)
    {
        selectImageName = button.name;
        pp.ChangeProfileImage(selectImageName);
    }

    public void OnClickResetButton()
    {
        pp.ChangeProfileImage("Reset");
    }
}
