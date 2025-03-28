using System.Buffers;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text playerlevel;
    [SerializeField] private TMP_Text levelPoint;
    
   

    public void SetGameOverPanel(Piece.Owner owner)
    {
        // 1. LOSE OR WIN text 고르기
        if (owner == Piece.Owner.PLAYER_A)
        {
            winText.text = "WIN!!";
            winText.color = new Color(0.99f, 0.15f, 0.42f);
        }

        winText.text = "LOSE..";
        winText.color = new Color(0f, 0.18f, 1f);

        // 2. ...에서 playerLevel, levelPoint 가져오기 / 텍스트 변경

    }

    public void OnClickRestartButton()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickHomeButton()
    {
        SceneManager.LoadScene("Main");
    }
}
