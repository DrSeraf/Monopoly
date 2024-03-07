using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageCardUI : MonoBehaviour
{

    [SerializeField] Image colorField;
    [SerializeField] TMP_Text propertyName;
    [SerializeField] GameObject[] buildings;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text mortgageValueText;
    [SerializeField] Button mortgageButton, unMortgageButton;

    Player playerReference;
    MonopolyNode nodeReference;
    //Color setColor, int numberOFBuildings, bool isMortgage, int mortgageValue
    public void SetCard(MonopolyNode node, Player owner)
    {
        nodeReference = node;
        playerReference = owner;
        //Set Color
        if (node.propertyColorField != null)
        {
            colorField.color = node.propertyColorField.color;
            propertyName.text = node.name;
        }
        else
        {
            colorField.color = Color.black;
        }
        //Show buildings
        if (node.NumberOfHouses < 4)
        {
            for (int i = 0; i < node.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
        //Show mortgage image
        mortgageImage.SetActive(node.IsMortgaged);
        //Text Update
        mortgageValueText.text = "Стоимость залога <br> М " + node.MortgageValue;
        //Buttons
        mortgageButton.interactable = !node.IsMortgaged;
        unMortgageButton.interactable = node.IsMortgaged;
    }

    public void MortgageButton()
    {
        if (nodeReference.IsMortgaged)
        {
            //Error messagge or such
            return;
        }
        playerReference.CollectMoney(nodeReference.MortgageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
    }

    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            //Error messagge or such
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        mortgageButton.interactable = true;
        unMortgageButton.interactable = false;
    }
}
