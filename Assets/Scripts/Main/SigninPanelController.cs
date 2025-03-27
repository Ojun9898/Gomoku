using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField Username;
    [SerializeField] private TMP_InputField Password;
    
    public void OnClickSigninButton()
    {
        // string username = Username.text;
        // string password = Password.text;
        //
        // if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        // {
        //     LoginManager.Instance.ShowErrorPanel("모든 항목을\n입력해주세요.");
        //     return;
        // }
        //
        // LoginUser(username, password);
        SceneManager.LoadScene("Main");
    }

    public void OnClickSignupButton()
    {
        LoginManager.Instance.CloseSigninPanel();
        LoginManager.Instance.ShowSignupPanel();
    }

    public void LoginUser(string username, string password)
    {
        // 로그인 요청
        LoginManager.Instance.AttemptLogin(username, password, (result) =>
        {
            if (result == 0)
            {
                LoginManager.Instance.ShowErrorPanel("아이디 또는 비밀번호가\n일치하지 않습니다.");
                return;
            }
            else
            {
                LoginManager.Instance.CloseSigninPanel();
                SceneManager.LoadScene("Main");
            }
        });
    }

}
