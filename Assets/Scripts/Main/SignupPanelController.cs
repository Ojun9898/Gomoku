using System;
using System.IO;
using TMPro;
using UnityEngine;

public class SignupPanelController : MonoBehaviour
{
    [SerializeField] private TMP_InputField NicknameInputField;
    [SerializeField] private TMP_InputField UsernameInputField;
    [SerializeField] private TMP_InputField PasswordInputField;
    [SerializeField] private string filePath;

    private static readonly string[] BannedChars = { ",", "<", ">", "{", "}", "[", "]", "(", ")"};

    private void Start()
    {
        // filePath가 비어 있으면 경로를 지정
        if (string.IsNullOrEmpty(filePath))
        {
            filePath = Path.Combine(Application.dataPath, "Data", "UserInfo.csv");
        }

        // 경로가 올바른지 확인
        if (string.IsNullOrEmpty(filePath) || !Directory.Exists(Path.GetDirectoryName(filePath)))
        {
            Debug.LogError("Invalid file path: " + filePath);
            return;
        }

        // CSV 파일이 없으면 헤더 추가
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, "Date,Username,Password,Nickname,Score,PlayerLevel,LevelPoint\n");
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
        float score = 0;

        // 글자수 제한
        if (nickname.Length < 3 || username.Length < 3 || password.Length < 3)
        {
            LoginManager.Instance.ShowErrorPanel("3자 이상 입력해주세요.");
            return;
        }
        
        // 기존에 동일한 ID가 있는지 확인
        if (IsUsernameExists(username))
        {
            LoginManager.Instance.ShowErrorPanel("이미 존재하는\nID입니다.");
            return;
        }

        // 비밀번호에 사용할 수 없는 문자가 포함되어 있는지 확인
        foreach (var bannedChar in BannedChars)
        {
            if (password.Contains(bannedChar))
            {
                LoginManager.Instance.ShowErrorPanel("비밀번호에 사용할 수 없는\n문자가 포함되어있습니다.");
                return;
            }
        }   

        // 기존에 동일한 닉네임이 있는지 확인
        if (IsNicknameExists(nickname))
        {
            LoginManager.Instance.ShowErrorPanel("이미 존재하는\n닉네임입니다.");
            return;
        }

        // 현재 날짜 가져오기
        string date = DateTime.Now.ToString("yyyy-MM-dd");
        int playerLevel = 18;
        int levelPoint = 0;

        // 새로운 유저 데이터 추가
        string newEntry = $"{date},{username},{password},{nickname},{score},{playerLevel}, {levelPoint}\n";
        File.AppendAllText(filePath, newEntry);

        LoginManager.Instance.ShowErrorPanel("회원가입이\n완료되었습니다.");

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
