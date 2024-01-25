using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] TMP_Text playerNameText;
    [SerializeField] TMP_Text playerCashText;

    public void SetPlayerName(string newName)
    {
        playerNameText.text = newName;
    }

    public void SetCashName(int currentCash)
    {
        playerCashText.text = "$ " + currentCash;
    }

    public void SetPlayerNameAndCash(string newName, int currentCash)
    {
        SetPlayerName(newName);
        SetCashName(currentCash);
    }    

}
