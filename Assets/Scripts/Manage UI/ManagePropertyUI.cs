using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ManagePropertyUI : MonoBehaviour
{
    [SerializeField] Transform cardHolder;//Horizontal layout
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Button buyHouseButton, sellHouseButton;
    [SerializeField] TMP_Text buyHousePriceText, sellHousePriceText;
    Player playerReference;
    List<MonopolyNode> nodesInSet = new List<MonopolyNode>();
    List<GameObject> cardsInSet = new List<GameObject>();

    //This property is only for 1 specific card set
    public void SetProperty(List<MonopolyNode> nodes, Player owner)
    {
        playerReference = owner;
        nodesInSet.AddRange(nodes);
        for (int i = 0; i < nodesInSet.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardHolder, false);
            ManageCardUI manageCardUI = newCard.GetComponent<ManageCardUI>();
            cardsInSet.Add(newCard);
            manageCardUI.SetCard(nodesInSet[i], owner);
        }

        var (list, allsame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        buyHouseButton.interactable = allsame;
        sellHouseButton.interactable = allsame;
    }

    public void BuyHouseButton()
    {
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuilsHouseOrHotelEvenly(nodesInSet);
            //Update money text
        }
        else
        {
            //Cant afford house - system message for player
        }
    }
    public void SellHouseButton()
    {//Check of there is at list 1 house to sell
        playerReference.SellHouseEvenly(nodesInSet);
        //Update money text
    }
}
