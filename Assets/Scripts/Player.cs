using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Player 
{

    public enum PlayerType
    {
        Human,
        AI
    }
    public PlayerType playerType;
    public string name;
    int money;
    MonopolyNode currentNode;
    bool isInJail;
    int numTurnsInJail;
    [SerializeField] GameObject myToken;
    [SerializeField] List<MonopolyNode> myMonopolyNodes = new List<MonopolyNode>();

    //Player Info
    PlayerInfo myInfo;

    //AI
    int aiMoneySafaty = 200;


    //Return some infos
    public bool IsInJail => isInJail;
    public GameObject MyToken => myToken;
    public MonopolyNode MyCurrentMonopolyNode => currentNode;

    public void Inititialize(MonopolyNode startNode, int startingMoney, PlayerInfo info, GameObject token)
    {
        currentNode = startNode;
        money = startingMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
    }
}
