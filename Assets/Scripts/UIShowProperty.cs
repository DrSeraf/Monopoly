using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIShowProperty : MonoBehaviour
{

    MonopolyNode nodeReference;

    [Header("Buy Property UI")]
    [SerializeField] GameObject propertyUIPanel;
    [SerializeField] TMP_Text propertyNameText;
    [SerializeField] Image colorField;
    [Space]
    [SerializeField] TMP_Text rentPriceTExt; //Without house
    [SerializeField] TMP_Text oneHouseRentText;
    [SerializeField] TMP_Text twoHouseRentText;
    [SerializeField] TMP_Text threeHouseRentText;
    [SerializeField] TMP_Text foutHouseRentText;
    [SerializeField] TMP_Text hotelRentText;
    [Space]
    [SerializeField] TMP_Text housePriceText;
    [SerializeField] TMP_Text hotelPriceText;
    [Space]
    [SerializeField] Button buyPropertyButton;
    [Space]
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] TMP_Text playerMoneyText;


    void ShowBuyPropertyUI(MonopolyNode node, Player currentPlayer)
    {
        nodeReference = node;
        //Top panel content
        propertyNameText.text = node.name;
        colorField.color = node.propertyColorField.color;
    }


}
