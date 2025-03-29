using System;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField NicknameInputField;
    [SerializeField] private TMP_InputField UsernameInputField;
    [SerializeField] private TMP_InputField PasswordInputField;

    private string filePath;

    private static readonly string[] BannedChars = { ",", "<", ">", "{", "}", "[", "]", "(", ")" };

    private void Start()
    {
        // StreamingAssets 경로 지정
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "UserInfo.csv");
        string persistentPath = Path.Combine(Application.persistentDataPath, "UserInfo.csv");

        // 파일이 없으면 StreamingAssets에서 PersistentDataPath로 복사
        if (!File.Exists(persistentPath))
        {
            #if UNITY_ANDROID
            StartCoroutine(CopyFileFromStreamingAssets(streamingPath, persistentPath));
            #else
            if (File.Exists(streamingPath))
            {
                File.Copy(streamingPath, persistentPath, true);
            }
            #endif
        }

        filePath = persistentPath;
    }

    private IEnumerator CopyFileFromStreamingAssets(string sourcePath, string destPath)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(sourcePath))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(destPath, request.downloadHandler.data);
            }
            else
            {
                Debug.LogError("파일 복사 실패: " + request.error);
            }
        }
    }
    public void OnClickSigninButton()
    {
        LoginManager.Instance.CloseSignupPanel();
        LoginManager.Instance.ShowSigninPanel();
    }
    public void OnClickSignupButton()
    {
        string nickname = NicknameInputField.text;
        string username = UsernameInputField.text;
        string password = PasswordInputField.text;

        if (string.IsNullOrEmpty(nickname) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            MainManager.Instance.ShowErrorPanel("모든 항목을\n입력해주세요.");
            return;
        }

        RegisterUser(nickname, username, password);
    }

    public void RegisterUser(string nickname, string username, string password)
    {
        if (nickname.Length < 3 || username.Length < 3 || password.Length < 3)
        {
            MainManager.Instance.ShowErrorPanel("3자 이상 입력해주세요.");
            return;
        }

        if (IsUsernameExists(username))
        {
            MainManager.Instance.ShowErrorPanel("이미 존재하는\nID입니다.");
            return;
        }

        foreach (var bannedChar in BannedChars)
        {
            if (password.Contains(bannedChar))
            {
                MainManager.Instance.ShowErrorPanel("비밀번호에 사용할 수 없는\n문자가 포함되어있습니다.");
                return;
            }
        }

        if (IsNicknameExists(nickname))
        {
            MainManager.Instance.ShowErrorPanel("이미 존재하는\n닉네임입니다.");
            return;
        }

        string date = DateTime.Now.ToString("yyyy-MM-dd");
        int playerLevel = 0;
        int levelPoint = 0;
        float score = 0;

        string newEntry = $"{date},{username},{password},{nickname},{score},{playerLevel},{levelPoint}\n";
        File.AppendAllText(filePath, newEntry);

        MainManager.Instance.ShowErrorPanel("회원가입이\n완료되었습니다.");

        LoginManager.Instance.CloseSignupPanel();
        LoginManager.Instance.ShowSigninPanel();
    }

    private bool IsUsernameExists(string username)
    {
        if (!File.Exists(filePath)) return false;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            if (data.Length > 1 && data[1] == username)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsNicknameExists(string nickname)
    {
        if (!File.Exists(filePath)) return false;

        string[] lines = File.ReadAllLines(filePath);
        foreach (string line in lines)
        {
            string[] data = line.Split(',');
            if (data.Length > 3 && data[3] == nickname)
            {
                return true;
            }
        }
        return false;
    }
}
