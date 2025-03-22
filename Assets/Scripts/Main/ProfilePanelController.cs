using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ProfilePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text Username;
    [SerializeField] private TMP_Text Nickname;
    [SerializeField] private TMP_Text Date;
    [SerializeField] private TMP_Text Score;

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
        Score.text = UserInfo[4];
    }

     public void OnClickCloseButton()
    {
        this.GetComponent<RectTransform>().DOLocalMoveX(-600f, 0.3f)
            .OnComplete(() => this.gameObject.SetActive(false)); 
    }
}
