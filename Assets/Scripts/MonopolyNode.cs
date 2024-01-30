// Используемые пространства имен
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Properties;
using System.Xml.Serialization;

// Перечисление для типов узлов в игре монополии
public enum MonopolyNodeType
{
    Property,
    Utility,
    RailRoad,
    Tax,
    Chance,
    CommunityChest,
    Go,
    Jail,
    FreeParking,
    GoToJail
}

// Класс MonopolyNode, который представляет собой узел в игре монополии
public class MonopolyNode : MonoBehaviour
{
    // Тип узла
    public MonopolyNodeType monopolyNodeType;

    // Имя узла
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;

    // Цена узла
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;

    // Арендная плата узла
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal int[] rentWithHouse;

    // Залог узла
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;

    // Владелец узла
    [Header("Property Owner")]
    public Player owner;
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;

    // Метод, который вызывается при изменении значений в инспекторе Unity
    private void OnValidate()
    {
        // Обновление текста имени
        if (nameText != null)
        {
            nameText.text = name;
        }

        // Расчет цены и арендной платы
        if (calculateRentAuto)
        {
            // Расчет цены и арендной платы для типа Property
            if (monopolyNodeType == MonopolyNodeType.Property)
            {
                if(baseRent > 0)
                {
                    price = 3 * (baseRent * 10);

                    // Расчет стоимости залога
                    mortgageValue = price/2;

                    // Расчет арендной платы с домом
                    rentWithHouse = new int[]
                    {
                        baseRent * 5, 
                        baseRent * 5 * 3,
                        baseRent * 5 * 9,
                        baseRent * 5 * 16,
                        baseRent * 5 * 25,
                    };
                }
            }

            // Расчет стоимости залога для типов Utility и RailRoad
            if (monopolyNodeType == MonopolyNodeType.Utility)
            {
                mortgageValue = price / 2;
            }

            if (monopolyNodeType == MonopolyNodeType.RailRoad)
            {
                mortgageValue = price / 2;
            }
        }

        // Обновление текста цены
        if (priceText != null)
        {
            priceText.text = "$ " + price;
        }
        //Update the owner
        OnOwnerUpdated();
        UnMortgageProperty();
        //isMortgaged = false;
    }

    // Метод для залога свойства
    public int MortgageProperty()
    {
        isMortgaged = true;
        if (mortgageImage != null)
        {
            mortgageImage.SetActive(true);
        }
        if (propertyImage != null)
        {
            propertyImage.SetActive(false);
        }
        return mortgageValue;
    }

    // Метод для освобождения залога с свойства
    public void UnMortgageProperty()
    {
        isMortgaged = false;
        if(mortgageImage != null) 
        {
            mortgageImage.SetActive(false);
        }
        if (propertyImage != null)
        {
            propertyImage.SetActive(true);
        }
        
    }

    // Метод для обновления владельца
    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;

    //Update owner
    public void OnOwnerUpdated()
    {
        if(ownerBar != null)
        {
            if(owner.name != "")
            {
                ownerBar.SetActive(true);
                ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }
        }
    }


    public void PlayerLandenOnNode(Player currentPlayer)
    {
        bool playerIsHuman = currentPlayer.playerType == Player.PlayerType.Human;

        //Check for node type and act

        switch(monopolyNodeType)
        {
            case MonopolyNodeType.Property:

            break;

            case MonopolyNodeType.Utility:

            break;

            case MonopolyNodeType.RailRoad:

            break;

            case MonopolyNodeType.Tax:

            break;

            case MonopolyNodeType.FreeParking:

            break;

            case MonopolyNodeType.GoToJail:

            break;

            case MonopolyNodeType.Chance:

            break;

            case MonopolyNodeType.CommunityChest:

            break;

        }





        //Continue
        if (!playerIsHuman) 
        {
            Invoke("ContinueGame", 2f);
        }
        else 
        {
            //show UI
        }
    }

    void ContinueGame()
    {
        //If the last roll was not a double 
        //roll agaid

        //not a double roll
        //switch player
        GameManager.instance.SwitchPlayer();
    }

}
