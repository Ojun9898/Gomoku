using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public struct SigninData
{
    public string id;
    public string password;
}

public struct SigninResult
{
    public int result;
}
public class SigninPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField ID;
    [SerializeField] private TMP_InputField Password;
    
    public void OnClickSigninButton()
    {
        string id = ID.text;
        string password = Password.text;

        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            // 입력창 오류 팝업
            return;
        }

        var signinData = new SigninData();
        signinData.id = id;
        signinData.password = password;

        // StartCoroutine(NetworkManage.Instance.Signin(signinData, () =>
        // {
        //     Destroy(gameObject);
        // }, result =>
        // {
        //     if (result == 0)
        //     {
        //         ID.text = "";
        //     }
        //     else if (result == 1)
        //     {
        //         Password.text = "";
        //     }
        // }));
    }

    public void OnClickSignupButton()
    {
        Debug.Log("Signup button clicked!");
    }


}
