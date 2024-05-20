using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TradePropertyCard : MonoBehaviour
{
    MonopolyNode nodeReference;

    [SerializeField] Image colorField;
    [SerializeField] TMP_Text propertyNameText;
    //[SerializeField] Image typeImage;
    //[SerializeField] Sprite houseSprite;
    //[SerializeField] Sprite railRoadSprite;
    //[SerializeField] Sprite utilitySprite;
    [SerializeField] GameObject mortgageImage;
    [SerializeField] TMP_Text propertyPriceText;
    [SerializeField] Toggle toggleButton;

    public void SetTradeCard(MonopolyNode node, ToggleGroup toggleGroup)
    {
        nodeReference = node;
        colorField.color = (node.propertyColorField != null)?node.propertyColorField.color : Color.white;
        propertyNameText.text = node.name;
        /*switch(node.monopolyNodeType)
        {
            case MonopolyNodeType.Property:
                typeImage.sprite = houseSprite;
                typeImage.color = Color.white;
            break;
        }*/
        mortgageImage.SetActive(node.IsMortgaged);
        propertyPriceText.text = "M " + node.price;
        toggleButton.isOn = false;
        toggleButton.group = toggleGroup;

    }
    public MonopolyNode Node()
    {
        return nodeReference;
    }
}
