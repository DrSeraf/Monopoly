using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageSystem : MonoBehaviour
{

    [SerializeField] TMP_Text messageText;

    private void OnEnable()
    {
        ClearMessage();
        GameManager.OnUpdateMessage += ReceiveMessage;
        Player.OnUpdateMessage += ReceiveMessage;
        MonopolyNode.OnUpdateMessage += ReceiveMessage;
        TradingSystem.OnUpdateMessage += ReceiveMessage;   
    }

    private void OnDisable()
    {
        GameManager.OnUpdateMessage -= ReceiveMessage;
        Player.OnUpdateMessage -= ReceiveMessage;
        MonopolyNode.OnUpdateMessage -= ReceiveMessage;
        TradingSystem.OnUpdateMessage -= ReceiveMessage;
    }

    void ReceiveMessage(string _message)
    {
        messageText.text = _message;
        //Invoke(nameof(ClearMessage), 5f);
    }

    void ClearMessage()
    {
        messageText.text = "";
    }

}
