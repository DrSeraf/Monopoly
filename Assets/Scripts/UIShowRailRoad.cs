using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIShowRailRoad : MonoBehaviour
{

    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy RailRoad UI")]
    [SerializeField] GameObject railroadUIPanel;
    [SerializeField] TMP_Text railroadText;
    //[SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text oneRailroadRentText;
    [SerializeField] TMP_Text twoRailroadRentText;
    [SerializeField] TMP_Text threeRailroadRentText;
    [SerializeField] TMP_Text foutRailroadRentText;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyRailRoadButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    private void OnEnable()
    {
        MonopolyNode.OnShowRailRoadBuyPanel += ShowBuyRailRaodUI;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowRailRoadBuyPanel -= ShowBuyRailRaodUI;
    }

    private void Start()
    {
        railroadUIPanel.SetActive(false);
    }

    private void ShowBuyRailRaodUI(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //Top panel content
        railroadText.text = node.name;
        //colorField.color = node.propertyColorField.color;
        //Center of the card
        //result = baseRent * (int)Mathf.Pow(2, amount - 1);
        oneRailroadRentText.text = "М " + node.baseRent * (int)Mathf.Pow(2, 1 - 1);
        twoRailroadRentText.text = "М " + node.baseRent * (int)Mathf.Pow(2, 2 - 1);
        threeRailroadRentText.text = "М " + node.baseRent * (int)Mathf.Pow(2, 3 - 1);
        foutRailroadRentText.text = "М " + node.baseRent * (int)Mathf.Pow(2, 4 - 1);
        //Cost of buildings
        mortgagePriceText.text = "М " + node.MortgageValue;
        //Bottom bar
        propertyPriceText.text = "М " + node.price;
        playerMoneyText.text = "У вас: М " + currentPlayer.ReadMoney;
        //Buy property button
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyRailRoadButton.interactable = true;
        }
        else
        {
            buyRailRoadButton.interactable = false;
        }
        //Show the panel
        railroadUIPanel.SetActive(true);
    }
    public void BuyRailRoadButton()//This is called from by button
    {
        //Tell the player to get purchase
        playerReference.BuyProperty(nodeReference);
        //Maybeclose the property card

        //Make the button not interacteble
        buyRailRoadButton.interactable = false;
    }
    public void CloseRailRoadButton()//This is called from by button
    {
        //Close the panel
        railroadUIPanel.SetActive(false);
        //Clear node regerence 
        nodeReference = null;
        playerReference = null;
    }

}
