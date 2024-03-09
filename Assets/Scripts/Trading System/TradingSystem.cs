using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using System.Linq;

public class TradingSystem : MonoBehaviour
{
    //Message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    public static TradingSystem instance;
    private void Awake()
    {
        instance = this;
    }
    //-------------------------------FIND MISSING PROPERTYS IN SET-------------------------AI

    public void FindMissingProperty(Player currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestedNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            //CHECK IF ALL HAVE BEEN PURCHASED
            bool notAllPurchased = list.Any(n => n.Owner == null);
            if (allSame || processedSet == list || notAllPurchased)
            {
                processedSet = nodeSet;
                continue;
            }
            //FIND THE OWNER BY OTHER PLAYER

            //BUT CHECK IF WE HAVE MORE THE THE AVERAGE
            if (list.Count == 2)
            {
                requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if (requestedNode != null)
                {
                    //MAKE OFFER TO OWNER THE NODE
                    break;
                }
            }
            if (list.Count >= 3)
            {
                int hasMostOFSet = list.Count(n => n.Owner == currentPlayer);
                if (hasMostOFSet >= 2)
                {
                    requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    break;
                }
            }
        }
    }

    //-------------------------------MAKE TRADE DECISION-------------------------------------
    void MakeTradeDecision(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode)
    {
        //TRADE WITH MONEY IF POSSIBLE
        if (currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            //TRADE WITH MONEY ONLY 

            //MAKE TRADE OFFER
            MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }

        //FIND ALL INCOMPLETE SET AND EXCLUDE THE SET WITCH THE REQUESTED NODE
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node).list;
            if (checkedSet.Contains(requestedNode))
            {
                //STOP CHECKING HERE
                continue;
            }
            //VALID NODE CHECK
            if (checkedSet.Count(n => n.Owner == currentPlayer) == 1)//VALID NODE CHECK
            {
                if (CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int offeredMoney = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    //VALID TRADE POSSIBLE
                    MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, offeredMoney, 0);
                    //MAKE TRADE OFFER
                    break;
                }
            }
        }

        //FIND OUT IF ONLY ONE NODE OF THE FOUND SET IS OWNED

        //CALCULATE THE VALUE OF THAT NODE AND SEE IF WITH ENOUTH MONEY IT COULD BE AFFORDABLE

        //IF SO... MAKE TRADE OFFER
    }

    //-------------------------------MAKE TRADE OFFER----------------------------------------

    void MakeTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        if (nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else if (nodeOwner.playerType == Player.PlayerType.Human)
        {
            //SHOW UI
        }
    }

    //-------------------------------CONCIDER TRADE OFFER----------------------------------AI
    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = CalculateValueOfNode(requestedNode) + offeredMoney - requestedMoney - CalculateValueOfNode(offeredNode);
        if (valueOfTheTrade >= 0)
        {
            //TRADE THE NODE IS VALID
        }
        else
        {
            //DEBUG LINE OR TELL PLAYER THAT REJECTED
            Debug.Log("AI rejected trage offer");
        }
    }
    //-------------------------------CALCULATE THE VALUE OF NODE---------------------------AI

    int CalculateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if (requestedNode != null)
        {
            if (requestedNode.monopolyNodeType == MonopolyNodeType.Property)
            {
                value = requestedNode.price + requestedNode.NumberOfHouses * requestedNode.houseCost;
            }
            else
            {
                value = requestedNode.price;
            }
            return value;
        }
        return value;
    }

    //-------------------------------TRADE THE NODE------------------------------------------
    void Trade(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        //CURRENT PLAYER NEEDS TO
        if (requestedNode != null)
        {
            currentPlayer.PayMoney(offeredMoney);
            requestedNode.ChangeOwner(currentPlayer);
            //NODE OWNER
            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);
            if (offeredNode != null)
            {
                offeredNode.ChangeOwner(nodeOwner);
            }
            //SHOW A MESSAGE FOR THE UI
            OnUpdateMessage.Invoke(currentPlayer.name + " обменял " + requestedNode.name + " за " + offeredMoney + " & " + offeredNode.name + " игроку " + nodeOwner.name);
        }
        else if (offeredNode != null)
        {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offeredNode.ChangeOwner(nodeOwner);
            //SHOW A MESSAGE FOR THE UI
            OnUpdateMessage.Invoke(currentPlayer.name + " продал " + offeredNode.name + " игроку " + nodeOwner.name + " за " + requestedMoney);
        }
    }

}
