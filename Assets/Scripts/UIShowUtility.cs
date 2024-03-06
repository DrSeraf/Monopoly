using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIShowUtility : MonoBehaviour
{

    MonopolyNode nodeReference;
    Player playerReference;

    [Header("Buy Utility UI")]
    [SerializeField] GameObject utilityUIPanel;
    [SerializeField] TMP_Text utilityText;
    //[SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text mortgagePriceText;
    [Space]
    [SerializeField] Button buyUtilityButton;
    [Space]
    [SerializeField] TMP_Text utilityPriceText;
    [SerializeField] TMP_Text playerMoneyText;

    private void OnEnable()
    {
        MonopolyNode.OnShowUtilityBuyPanel += ShowBuyUtilityUI;
    }

    private void OnDisable()
    {
        MonopolyNode.OnShowUtilityBuyPanel -= ShowBuyUtilityUI;
    }

    private void Start()
    {
        utilityUIPanel.SetActive(false);
    }

    private void ShowBuyUtilityUI(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        playerReference = currentPlayer;
        //Top panel content
        utilityText.text = node.name;
        //colorField.color = node.propertyColorField.color;
        //Cost of buildings
        mortgagePriceText.text = "М " + node.MortgageValue;
        //Bottom bar
        utilityPriceText.text = "Цена: М " + node.price;
        playerMoneyText.text = "У вас: М " + currentPlayer.ReadMoney;
        //Buy property button
        if (currentPlayer.CanAffordNode(node.price))
        {
            buyUtilityButton.interactable = true;
        }
        else
        {
            buyUtilityButton.interactable = false;
        }
        //Show the panel
        utilityUIPanel.SetActive(true);
    }
    public void BuyUtilityButton()//This is called from by button
    {
        //Tell the player to get purchase
        playerReference.BuyProperty(nodeReference);
        //Maybeclose the property card

        //Make the button not interacteble
        buyUtilityButton.interactable = false;
    }
    public void CloseUtilityButton()//This is called from by button
    {
        //Close the panel
        utilityUIPanel.SetActive(false);
        //Clear node regerence 
        nodeReference = null;
        playerReference = null;
    }
}
