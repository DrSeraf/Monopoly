using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIShowProperty : MonoBehaviour
{

    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUIPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceText; //Without house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text foutHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    private void OnEnable()
    {
        MonopolyNode.OnShowPropertyBuyPanel += ShowBuyPropertyUI;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowPropertyBuyPanel -= ShowBuyPropertyUI;
    }

    private void Start()
    {
        propertyUIPanel.SetActive(false);
    }

    void ShowBuyPropertyUI(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //Top panel content
        propertyNameText.text = node.name;
        colorField.color = node.propertyColorField.color;
        //Center of the card
        rentPriceText.text = "М " + node.baseRent;
        oneHouseRentText.text = "М " + node.rentWithHouse[0];
        twoHouseRentText.text = "М " + node.rentWithHouse[1];
        threeHouseRentText.text = "М " + node.rentWithHouse[2];
        foutHouseRentText.text = "М " + node.rentWithHouse[3];
        hotelRentText.text = "М " + node.rentWithHouse[4];
        //Cost of buildings
        housePriceText.text = "М " + node.houseCost;
        mortgagePriceText.text = "М " + node.MortgageValue;
        //Bottom bar
        propertyPriceText.text = "М " + node.price;
        playerMoneyText.text = "У вас: М " + currentPlayer.ReadMoney;
        //Buy property button
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyPropertyButton.interactable = true;
        }
        else
        {
            buyPropertyButton.interactable = false;
        }
        //Show the panel
        propertyUIPanel.SetActive(true);
    }

    public void BuyPropertyButton()//This is called from by button
    {
        //Tell the player to get purchase
        playerReference.BuyProperty(nodeReference);
        //Maybeclose the property card

        //Make the button not interacteble
        buyPropertyButton.interactable = false;
    }
    public void ClosePropertyButton()//This is called from by button
    {
        //Close the panel
        propertyUIPanel.SetActive(false);
        //Clear node regerence 
        nodeReference = null;
        playerReference = null;
    }

}
