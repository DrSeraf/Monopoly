using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.GraphView;

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
    ManagePropertyUI propertyReference;

    //Color setColor, int numberOFBuildings, bool isMortgage, int mortgageValue
    public void SetCard(MonopolyNode node, Player owner, ManagePropertyUI propertySet)
    {
        nodeReference = node;
        playerReference = owner;
        propertyReference = propertySet;
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
        ShowBuildings();
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
        if (!propertyReference.CheckIfMortgageAllowed())
        {
            //Error messagge or such
            string message = "У вас стоит зелье на имуществе! Вы не можете его заложить";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (nodeReference.IsMortgaged)
        {
            //Error messagge or such
            string message = "Уже под залогом!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.CollectMoney(nodeReference.MortgageProperty());
        mortgageImage.SetActive(true);
        mortgageButton.interactable = false;
        unMortgageButton.interactable = true;
        ManageUI.instance.UpdateMoneyText();
    }

    public void UnMortgageButton()
    {
        if (!nodeReference.IsMortgaged)
        {
            //Error messagge or such
            string message = "Уже под выкуплено!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        if (playerReference.ReadMoney < nodeReference.MortgageValue)
        {
            //Error messagge or such
            string message = "У вас недостоточно денег!";
            ManageUI.instance.UpdateSystemMessage(message);
            return;
        }
        playerReference.PayMoney(nodeReference.MortgageValue);
        nodeReference.UnMortgageProperty();
        mortgageImage.SetActive(false);
        mortgageButton.interactable = true;
        unMortgageButton.interactable = false;
        ManageUI.instance.UpdateMoneyText();
    }

    public void ShowBuildings()
    {
        //Hide all buildings first
        foreach (var icon in buildings)
        {
            icon.SetActive(false);
        }
        //Show buildings
        if (nodeReference.NumberOfHouses < 5)
        {
            for (int i = 0; i < nodeReference.NumberOfHouses; i++)
            {
                buildings[i].SetActive(true);
            }
        }
        else
        {
            buildings[4].SetActive(true);
        }
    }
}
