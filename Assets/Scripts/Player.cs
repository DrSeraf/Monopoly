using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.UI.GridLayoutGroup;

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
    int numTurnsInJail = 0;
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
    public int ReadMoney => money;

    //Message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    public void Inititialize(MonopolyNode startNode, int startingMoney, PlayerInfo info, GameObject token)
    {
        currentNode = startNode;
        money = startingMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
    }

    public void SetMyCurrentNode(MonopolyNode newNode)//Turn is over
    {
        currentNode = newNode;
        //Player landed on node so lets
        newNode.PlayerLandenOnNode(this);

        //if its AI player

        //Check if can bild houses

        //Check for anmortgage properties

        //Check if he could trade for missing properties
    }

    public void CollectMoney(int amount)
    {
        money += amount;
        myInfo.SetCashName(money);
    }

    internal bool CanAffordNode(int price)
    {
        return price <= money;
    }

    public void BuyProperty(MonopolyNode node)
    {
        money -= node.price;
        node.SetOwner(this);
        //Update UI
        myInfo.SetCashName(money);
        //Set ownership
        myMonopolyNodes.Add(node);
        //Sort all nodes by price
        SortPropertiesByPrice();

    }

    void SortPropertiesByPrice()
    {
        myMonopolyNodes.OrderBy(_node => _node.price).ToList();
    }

    internal void PayRent(int rentAmount, Player owner)
    {
        //Dpnt have enouth money
        if (money < rentAmount)
        {
            //Handle insufficent fund > AI
        }
        money -= rentAmount;
        owner.CollectMoney(rentAmount);

        //Update UI
        myInfo.SetCashName(money);
    }
    internal void PayMoney(int amount)
    {
        //Dpnt have enouth money
        if (money < amount)
        {
            //Handle insufficent fund > AI
        }
        money -= amount;
        //Update UI
        myInfo.SetCashName(money);
    }

    //----------------------------JAIL------------------------------------
    public void GoToJailVoid(int indexOnBoard)
    {
        isInJail = true;
        //Reposition player
        MonopolyBoard.Instance.MovePlayerToken(CalculateDistanceFromJail(indexOnBoard), this);
        GameManager.instance.RestRolledADouble();

    }

    int CalculateDistanceFromJail(int indexOnBoard)
    {
        int result = 0;
        int indexOfJail = 10;

        if (indexOnBoard > indexOfJail)
        {
            result = -(indexOnBoard - indexOfJail);
        }
        else
        {
            result = indexOfJail - indexOnBoard;
        }

        return result;
    }

    public IEnumerator GoToJail()
    {
        isInJail = true;
        //Reposition player
        yield return new WaitForSeconds(GameManager.instance.SecondsBetwinTurns);
        myToken.transform.position = MonopolyBoard.Instance.route[10].transform.position;
        currentNode = MonopolyBoard.Instance.route[10];
        GameManager.instance.RestRolledADouble();
    }
    public void SetOutOfJail()
    { 
        isInJail = false; 
        //Reset Turns In Jail
        numTurnsInJail = 0;
    }

    public int NumberTurnsInJail => numTurnsInJail;

    public void IncreaseNumTurnInJail()
    {
        numTurnsInJail++;
    }

    //-------------------------------HANDLE INSUFFICIENT FUNDS-------------------------------

    //-------------------------------BUNKRUPT-GAME-OVER--------------------------------------

    //-------------------------------UNMORTGAGE PROPERTY-------------------------------------

    //-------------------------------CHECK IF PLAYER HAS A PROPERTY SET----------------------

    //-------------------------------BUILD HOUSES EVENLY ON NODE SETS------------------------

    //-------------------------------TRADING SYSTEM------------------------------------------

    //-------------------------------FIND MISSING PROPERTYS IN SET---------------------------

    //-------------------------------HOUSES AND HOTELS - CAN AFFORT AND COUNT----------------

}
