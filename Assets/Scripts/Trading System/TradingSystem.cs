using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using System.Linq;

public class TradingSystem : MonoBehaviour
{
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

    //-------------------------------MAKE TRADE OFFER----------------------------------------

    //-------------------------------CONCIDER TRADE OFFER----------------------------------AI

    //-------------------------------CALCULATE THE VALUE OF NODE---------------------------AI

    //-------------------------------TRADE THE NODE------------------------------------------

    //-------------------------------REMOVE AND ADD NODES------------------------------------

}
