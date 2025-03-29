using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class ProfilePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text Username;
    [SerializeField] private TMP_Text Nickname;
    [SerializeField] private TMP_Text Date;
    [SerializeField] private TMP_Text PlayerLevel;


    private string[] UserInfo;

    void Start()
    {
        SettingProfile();
    }

    public void SettingProfile () 
    {
        UserInfo = LoginManager.Instance.GetUserInfo();

        Date.text = UserInfo[0];
        Username.text = UserInfo[1];
        Nickname.text = UserInfo[3];
        PlayerLevel.text = UserInfo[5];
    }

     public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }

    public void OnClickImageButton()
    {
        MainManager.Instance.ShowSelectPanel();
    }

}
