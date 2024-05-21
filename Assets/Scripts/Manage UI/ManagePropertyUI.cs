using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
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
            manageCardUI.SetCard(nodesInSet[i], owner, this);
        }

        var (list, allsame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(nodesInSet[0]);
        buyHouseButton.interactable = allsame && CheckIfBuyAllowed();
        sellHouseButton.interactable = CheckIfWeSellAllowed();


        buyHousePriceText.text = "-" + nodesInSet[0].houseCost;
        sellHousePriceText.text = "+" + nodesInSet[0].houseCost / 2;
    }

    public void BuyHouseButton()
    {
        if (!CheckIfBuyAllowed())
        {
            //Error message
            string message = "ќдно или несколько имуществ наход€тс€ под залогом, вы не можете поставить зелье.";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.CanAffordHouse(nodesInSet[0].houseCost))
        {
            playerReference.BuilsHouseOrHotelEvenly(nodesInSet);
            //Update money text
            string message = "¬ы поставили зелье.";
            ManageUI.instance.UpdateSystemMessage(message);
            UpdateHouseVisuals();
        }
        else
        {
            //Cant afford house - system message for player
            string message = "” вас недостаточно денег!";
            ManageUI.instance.UpdateSystemMessage(message);
        }
        sellHouseButton.interactable = CheckIfWeSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }
    public void SellHouseButton()
    {//Check of there is at list 1 house to sell
        playerReference.SellHouseEvenly(nodesInSet);
        //Update money text
        UpdateHouseVisuals();

        sellHouseButton.interactable = CheckIfWeSellAllowed();
        ManageUI.instance.UpdateMoneyText();
    }
    bool CheckIfWeSellAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool CheckIfBuyAllowed()
    {

        if (nodesInSet.Any(n => n.IsMortgaged) == true)
        {
            return false;
        }

        return true;
    }

    public bool CheckIfMortgageAllowed()
    {
        if (nodesInSet.Any(n => n.NumberOfHouses > 0))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void UpdateHouseVisuals()
    {
        foreach (var card in cardsInSet)
        {
            card.GetComponent<ManageCardUI>().ShowBuildings();
        }
    }
}
