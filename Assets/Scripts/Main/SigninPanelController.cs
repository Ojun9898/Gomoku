using System;
using TMPro;
using UnityEngine;

public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField Username;
    [SerializeField] private TMP_InputField Password;
    
    public void OnClickSigninButton()
    {
        string username = Username.text;
        string password = Password.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MainManager.Instance.ShowErrorPanel("모든 항목을\n입력해주세요.");
            return;
        }

        LoginUser(username, password);
    }

    public void OnClickSignupButton()
    {
        MainManager.Instance.CloseSigninPanel();
        MainManager.Instance.ShowSignupPanel();
    }

    public void LoginUser(string username, string password)
    {
        // 로그인 요청
        LoginManager.Instance.AttemptLogin(username, password, (result) =>
        {
            if (result == 0)
            {
                MainManager.Instance.ShowErrorPanel("아이디 또는 비밀번호가\n일치하지 않습니다.");
                return;
            }
            else
            {
                MainManager.Instance.CloseSigninPanel();
                MainManager.Instance.ShowErrorPanel("로그인 성공했습니다.");
                MainManager.Instance.ShowMainPanel();
            }
        });
    }

}
