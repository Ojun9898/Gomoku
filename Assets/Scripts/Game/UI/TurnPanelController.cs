using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TurnPanelController : MonoBehaviour
{
    [SerializeField] private TMP_Text TurnText;
    private string turn;

    public void ShowTurnText(Piece.Owner playerType)
    {
        if (playerType == Piece.Owner.PLAYER_A)
        {
            turn = "YOU";
        }

        else if (playerType == Piece.Owner.PLAYER_B)
        {
            turn = "COM";
        }

        else
        {
            turn = "ERROR";
        }

        TurnText.text = turn;
    }
}
