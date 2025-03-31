using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ProfilePanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text username;
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private TMP_Text date;
    [SerializeField] private TMP_Text playerLevel;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] unitSprites;

    private string[] UserInfo;
    private string csvPath;

    void Start()
    {
        csvPath = Path.Combine(Application.persistentDataPath, "Profile.csv");
        CopyCSVFromStreamingAssets();
        SettingProfile();
        LoadProfileImage();
    }

    public void SettingProfile()
    {
        UserInfo = LoginManager.Instance.GetUserInfo();
        date.text = UserInfo[0];
        username.text = UserInfo[1];
        nickname.text = UserInfo[3];
        playerLevel.text = UserInfo[5];
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

    public void ChangeProfileImage(string spriteName)
    {
        Sprite foundSprite = System.Array.Find(unitSprites, sprite => sprite.name == spriteName);
        if (foundSprite != null)
        {
            image.sprite = foundSprite;
            SaveProfileImage(spriteName);
        }
    }

    private void SaveProfileImage(string spriteName)
    {
        string usernameValue = LoginManager.Instance.GetUserInfo()[1]; // Get username from UserInfo
        string dataToSave = $"{usernameValue},{spriteName}"; // CSV 형식으로 저장 (username, spriteName)

        // CSV에 덧붙이기 전에 기존 내용을 확인
        string[] lines = File.Exists(csvPath) ? File.ReadAllLines(csvPath) : new string[0];

        bool isUserFound = false;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] data = lines[i].Split(',');

            if (data.Length == 2 && data[0] == usernameValue)
            {
                // 해당 username에 대한 데이터가 있으면 업데이트
                lines[i] = dataToSave;
                isUserFound = true;
                break;
            }
        }

        if (!isUserFound)
        {
            // username이 없다면 새로운 데이터 추가
            var newLines = new string[lines.Length + 1];
            lines.CopyTo(newLines, 0);
            newLines[lines.Length] = dataToSave;

            lines = newLines;
        }

        // 파일에 저장 (덮어쓰지 않고 모든 데이터를 기록)
        File.WriteAllLines(csvPath, lines);
    }

    private void LoadProfileImage()
    {
        if (File.Exists(csvPath))
        {
            string[] lines = File.ReadAllLines(csvPath);

            foreach (var line in lines)
            {
                string[] data = line.Split(',');

                if (data.Length == 2 && data[0] == LoginManager.Instance.GetUserInfo()[1]) // 로그인된 사용자와 비교
                {
                    string savedSpriteName = data[1];
                    ChangeProfileImage(savedSpriteName);
                    break;
                }
            }
        }
    }

    private void CopyCSVFromStreamingAssets()
    {
        string streamingPath = Path.Combine(Application.streamingAssetsPath, "Profile.csv");
        // 여기에 스트리밍 자산에서 파일을 복사하는 코드 추가 가능
    }
}
