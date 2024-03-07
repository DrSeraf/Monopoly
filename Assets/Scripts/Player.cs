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
    //Human input panel
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn);
    public static ShowHumanPanel OnShowHumanPanel;

    public void Inititialize(MonopolyNode startNode, int startingMoney, PlayerInfo info, GameObject token)
    {
        currentNode = startNode;
        money = startingMoney;
        myInfo = info;
        myInfo.SetPlayerNameAndCash(name, money);
        myToken = token;
        myInfo.ActivateArrow(false);
    }

    public void SetMyCurrentNode(MonopolyNode newNode)//Turn is over
    {
        currentNode = newNode;
        //Player landed on node so lets
        newNode.PlayerLandenOnNode(this);

        //if its AI player
        if (playerType == PlayerType.AI)
        {
            //Check if can bild houses
            CheckIfPlayerHasASet();
            //Check for anmortgage properties
            UnMortgageProperties();

            //Check if he could trade for missing properties
        }

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
            if (playerType == PlayerType.AI)
            {
                //Handle insufficent fund > AI
                HandleInsufficientFunds(rentAmount);
            }
            else
            {
                //Disable human turn and roll dice
                OnShowHumanPanel.Invoke(true, false, false);
            }
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
            if (playerType == PlayerType.AI)
            {
                //Handle insufficent fund > AI
                HandleInsufficientFunds(amount);
            }
            else 
            {
                //Disable human turn and roll dice
                OnShowHumanPanel.Invoke(true,false, false);
            }

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

    //-------------------------------STREET REPAIRS-----------------------------------------

    public int[] CountHousesAndHotels()
    {
        int houses = 0;//Goes to index 0
        int hotels = 0;//Goes to index 1

        foreach (var node in myMonopolyNodes)
        {
            if(node.NumberOfHouses != 5)
            {
                houses += node.NumberOfHouses;
            }
            else
            {
                hotels += 1;
            }
        }

        int[] allBuildings = new int[]{houses,hotels};
        return allBuildings;
    }

    //-------------------------------HANDLE INSUFFICIENT FUNDS-------------------------------

    void HandleInsufficientFunds(int amountToPay)
    {
        int housesToSell = 0;//Available houses to sell
        int allHouses = 0;
        int propertiesToMortgage = 0;
        int allPropertiesToMortgage = 0;

        //Count all houses
        foreach (var node in myMonopolyNodes)
        {
            allHouses += node.NumberOfHouses;
        }

        //Loop throw the properties and try to sell as much as needed
        while (money < amountToPay && allHouses > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                housesToSell = node.NumberOfHouses;
                if (housesToSell > 0)
                {
                    CollectMoney(node.SellHouseOrHotel());
                    allHouses--;
                    //Do we need more money?
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }
        //Mortgage
        foreach (var node in myMonopolyNodes)
        {
            allPropertiesToMortgage += (node.IsMortgaged) ? 0 : 1;
        }
        //Loop throght the properties and try to martgage as much as needed
        while (money < amountToPay && propertiesToMortgage > 0)
        {
            foreach (var node in myMonopolyNodes)
            {
                propertiesToMortgage = (!node.IsMortgaged) ? 1 : 0;
                if (propertiesToMortgage > 0)
                {
                    CollectMoney(node.MortgageProperty());
                    allPropertiesToMortgage--;
                    //Do we need more money?
                    if (money >= amountToPay)
                    {
                        return;
                    }
                }
            }
        }
        //We go bancrupt if we reach this point
        Bankrupt();
    }

    //-------------------------------BUNKRUPT-GAME-OVER--------------------------------------

    void Bankrupt()
    {
        //Take out the playre of the game



        //Send a message to message system

        OnUpdateMessage.Invoke(name + "Банкрот");

        //Clear all what the player has owned

        for (int i = myMonopolyNodes.Count - 1; i >= 0; i--)
        {
            myMonopolyNodes[i].ResetNode();
        }

        //Remove the player
        GameManager.instance.RemovePlayer(this);
    }

    public void RemoveProperty(MonopolyNode node)
    {
        myMonopolyNodes.Remove(node);
    }

    //-------------------------------UNMORTGAGE PROPERTY-------------------------------------

    void UnMortgageProperties()
    {
        //For AI
        foreach (var node in myMonopolyNodes)
        {
            if (node.IsMortgaged)
            {
                int cost = node.MortgageValue + (int)(node.MortgageValue * 0.1f);//10% interest
                //Can we afford to unmortgage
                if (money >= aiMoneySafaty + cost)
                {
                    PayMoney(cost);
                    node.UnMortgageProperty();
                }
            }
        }
    }

    //-------------------------------CHECK IF PLAYER HAS A PROPERTY SET----------------------

    void CheckIfPlayerHasASet()
    {
        //Call it only ones per set
        List<MonopolyNode> processedSet = null;
        //Store and compare
        foreach (var node in myMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node);
            if (!allSame)
            {
                continue;
            }

            List<MonopolyNode> nodeSet = list;
            if (nodeSet != null && nodeSet != processedSet)
            {
                bool hasMordgadedNode = nodeSet.Any(node => node.IsMortgaged)?true:false;
                if(!hasMordgadedNode)
                {
                    if (nodeSet[0].monopolyNodeType == MonopolyNodeType.Property)
                    {
                        //We could build a house on the set
                        BuilsHouseOrHotelEvenly(nodeSet);
                        //Update process set over her
                        processedSet = nodeSet;
                    }
                }
            }
        }
    }

    //-------------------------------BUILD HOUSES EVENLY ON NODE SETS------------------------

    internal void BuilsHouseOrHotelEvenly(List<MonopolyNode> nodesToBuildOn)
    {
        int minHouses = int.MaxValue;
        int maxHouses = int.MinValue;
        //Get min and max numbers of houses currently on the property
        foreach (var node in nodesToBuildOn)
        {
            int numOfHouses = node.NumberOfHouses;
            if (numOfHouses < minHouses)
            {
                minHouses = numOfHouses;
            }
            if (numOfHouses > maxHouses && numOfHouses < 5)
            {
                maxHouses = numOfHouses;
            }
        }
        //Buy houses on the properties for max allowed on the properties
        foreach (var node in nodesToBuildOn)
        {
            if (node.NumberOfHouses == minHouses && node.NumberOfHouses < 5 && CanAffordHouse(node.houseCost))
            {
                node.BuildHouseOrHotel();
                PayMoney(node.houseCost);
                //Stop the loop of it only should run ones
                break;
            }
        }

    }

    internal void SellHouseEvenly(List<MonopolyNode> nodesToSellFrom)
    {
        int minHouses = int.MaxValue;
        foreach (var node in nodesToSellFrom)
        {
            minHouses = Mathf.Min(minHouses, node.NumberOfHouses);
        }
        //Sell house
        for (int i = nodesToSellFrom.Count - 1; i >= 0; i--)
        {
            if (nodesToSellFrom[i].NumberOfHouses > minHouses)
            {
                CollectMoney(nodesToSellFrom[i].SellHouseOrHotel());
                break;
            }
        }
    }
    //-------------------------------TRADING SYSTEM------------------------------------------

    //-------------------------------FIND MISSING PROPERTYS IN SET---------------------------

    //-------------------------------HOUSES AND HOTELS - CAN AFFORT AND COUNT----------------

    internal bool CanAffordHouse(int price)
    {
        //Ai only
        if (playerType == PlayerType.AI)
        {
            return (money - aiMoneySafaty) >= price;
        }
        //Human only
        else
        {
            return (money) >= price;
        }
        
    }

    public void ActivateSelector(bool active)
    {
        myInfo.ActivateArrow(active);
    }

}
