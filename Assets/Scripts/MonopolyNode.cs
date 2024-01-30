// Используемые пространства имен
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Properties;
using System.Xml.Serialization;
using UnityEngine.UI;

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

    [SerializeField] Image propertyColorField;

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
    int numberOfHouses;

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

    public void UpdateColofield(Color color)
    {
        if (propertyColorField != null)
        {
            propertyColorField.color = color;
        }
        
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

                if(!playerIsHuman)//AI
                {
                    //If it owned && if we are not owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody

                        //Calculate current rent
                        int rentToPay = CalculatePropertyRent();

                        //Pay the rent to the owner

                        //Show a message about what happend

                    }
                    else if (owner.name == "")//&& if can afford
                    {
                        //Buy the node

                        //Show a message about what happend
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }
                else //Human
                {
                    //If it owned && if we are not owner && is not mortgaged
                    if (owner.name != "" && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody

                        //Calculate current rent

                        //Pay the rent to the owner

                        //Show a message about what happend

                    }
                    else if (owner.name == "")
                    {
                        //Show buy interface for the property
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }

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

    int CalculatePropertyRent()
    {
        switch(numberOfHouses)
        {
            case 0:
                //Check if owner hs the full set of nodes
                bool allSame = true;

                if(allSame)
                {
                    currentRent = baseRent * 2;
                }
                else
                {
                    currentRent = baseRent;
                }
                break;

            case 1:

                currentRent = rentWithHouse[0];

            break;

            case 2:

                currentRent = rentWithHouse[1];

            break;

            case 3:

                currentRent = rentWithHouse[2];

            break;

            case 4:

                currentRent = rentWithHouse[3];

            break;

            case 5://Hotel

                currentRent = rentWithHouse[4];

            break;
        }
        return currentRent;
    }

}
