using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField NicknameInputField;
    [SerializeField] private TMP_InputField IDInputField;
    [SerializeField] private TMP_InputField PasswordInputField;

    public void OnClickSignupButton()
    {
        string nickname = NicknameInputField.text;
        string id = IDInputField.text;
        string password = PasswordInputField.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
        {
            // 입력 요청 팝업
            return;
        }

        // 회원가입 요청

    }
}
