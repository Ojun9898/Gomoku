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

    public void ShowTurnText(string playerType)
    {
        if (playerType=="PlayerA")
        {
            turn = "YOU";
        }

        else if (playerType=="PlayerB")
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
