using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerTradeButton : MonoBehaviour
{
    Player playerReference;
    [SerializeField] TMP_Text playerNameText;

    public void SetPlayer(Player player)
    {
        playerReference = player;
    }

    public void SelectPlayer()
    {
        TradingSystem.instance.ShowRightPlayer(playerReference);
    }
}
