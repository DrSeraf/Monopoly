// Используемые пространства имен
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Properties;
using System.Xml.Serialization;
using UnityEngine.UI;
using System.Collections.ObjectModel;

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

public class MonopolyNode : MonoBehaviour
{
    // Тип узла
    public MonopolyNodeType monopolyNodeType;
    public Image propertyColorField; // Цвет недвижимости

    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText; // Текстовое поля для отображения имени узла

    [Header("Property Price")]
    public int price;
    public int houseCost;
    [SerializeField] TMP_Text priceText; // Текстовое поля для отображения цены узла

    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal List<int> rentWithHouse = new List<int>(); // Список арендных плат с домами
    int numberOfHouses; // Количество домов на узле
    public int NumberOfHouses => numberOfHouses;
    [SerializeField] GameObject[] houses;
    [SerializeField] GameObject hotel;
 
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage; // Изображение залога
    [SerializeField] GameObject propertyImage; // Изображение недвижимости
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;

    [Header("Property Owner")]
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText; // Текстовое поле для отображения владельца
    private Player owner;

    //Message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    //Drag a community card
    public delegate void DrawCommunityCard(Player player);
    public static DrawCommunityCard OnDrawCommunityCard;

    //Drag a chance card
    public delegate void DrawChanceCard(Player player);
    public static DrawChanceCard OnDrawChanceCard;

    //Human input panel 
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn);
    public static ShowHumanPanel OnShowHumanPanel;

    //Property buy panel
    public delegate void ShowPropertyBuyPanel(MonopolyNode node, Player player);
    public static ShowPropertyBuyPanel OnShowPropertyBuyPanel;

    //RailRoad buy panel
    public delegate void ShowRailRoadBuyPanel(MonopolyNode node, Player player);
    public static ShowRailRoadBuyPanel OnShowRailRoadBuyPanel;

    //Utility buy panel
    public delegate void ShowUtilityBuyPanel(MonopolyNode node, Player player);
    public static ShowUtilityBuyPanel OnShowUtilityBuyPanel;

    public Player Owner => owner; // Геттер для владельца узла
    public void SetOwner(Player newOwner)
    {
        owner = newOwner;
        OnOwnerUpdated();
    }


    // Метод, который вызывается при изменении значений в инспекторе Unity
    private void OnValidate()
    {
        // Обновление текста имени
        if (nameText != null)
        {
            nameText.text = name;
        }
        // Обновление текста цены
        if (priceText != null)
        {
            priceText.text = "М " + price;
        }
        // Расчет цены и арендной платы
        if (calculateRentAuto)
        {
            // Расчет цены и арендной платы для типа Property
            if (monopolyNodeType == MonopolyNodeType.Property)
            {
                if (baseRent > 0)
                {
                    price = 3 * (baseRent * 10);

                    // Расчет стоимости залога
                    mortgageValue = price / 2;

                    // Расчет арендной платы с домом
                    rentWithHouse.Clear();
                    rentWithHouse.Add(baseRent * 5);
                    rentWithHouse.Add(baseRent * 5 * 3);
                    rentWithHouse.Add(baseRent * 5 * 9);
                    rentWithHouse.Add(baseRent * 5 * 16);
                    rentWithHouse.Add(baseRent * 5 * 25);
                }
                else if(baseRent <= 0)
                {
                    price = 0;
                    baseRent = 0;
                    rentWithHouse.Clear();
                    mortgageValue = 0;
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
            if(owner != null)
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
        bool continueTurn = true;

        //Check for node type and act

        switch(monopolyNodeType)
        {
            case MonopolyNodeType.Property:

                if(!playerIsHuman)//AI
                {
                    //If it owned && if we are not owner && is not mortgaged
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody

                        //Calculate current rent
                        int rentToPay = CalculatePropertyRent();
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Show a message about what happend
                        OnUpdateMessage.Invoke(currentPlayer.name + " заплатил: " + rentToPay + " игроку: " + owner.name + " за аренду");

                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Buy the node
                        //Debug.Log("Player could buy");
                        OnUpdateMessage.Invoke(currentPlayer.name + " купил: " + this.name + " за: " + price);
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
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
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Calculate current rent
                        int rentToPay = CalculatePropertyRent();
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Pay the rent to the owner

                        //Show a message about what happend

                    }
                    else if (owner == null)
                    {

                        //Show buy interface for the property
                        OnShowPropertyBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }

            break;

            case MonopolyNodeType.Utility:
                if (!playerIsHuman)//AI
                {
                    //If it owned && if we are not owner && is not mortgaged
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody

                        //Calculate current rent
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Show a message about what happend
                        OnUpdateMessage.Invoke(currentPlayer.name + " заплатил: " + rentToPay + " игроку: " + owner.name + " за аренду");

                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Buy the node
                        //Debug.Log("Player could buy")
                        OnUpdateMessage.Invoke(currentPlayer.name + " купил: " + this.name + " за: " + price);
                        currentPlayer.BuyProperty(this);
                        //OnOwnerUpdated();
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
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Calculate current rent
                        int rentToPay = CalculateUtilityRent();
                        currentRent = rentToPay;
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Pay the rent to the owner

                        //Show a message about what happend

                    }
                    else if (owner == null)
                    {

                        //Show buy interface for the property
                        OnShowUtilityBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }

            break;

            case MonopolyNodeType.RailRoad:

                if (!playerIsHuman)//AI
                {
                    //If it owned && if we are not owner && is not mortgaged
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody

                        //Calculate current rent
                        int rentToPay = CalculateRailRoadRent();
                        currentRent = rentToPay;
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Show a message about what happend
                        OnUpdateMessage.Invoke(currentPlayer.name + " заплатил: " + rentToPay + " игроку: " + owner.name + " за аренду");

                    }
                    else if (owner == null && currentPlayer.CanAffordNode(price))
                    {
                        //Buy the node
                        //Debug.Log("Player could buy");
                        OnUpdateMessage.Invoke(currentPlayer.name + " купил: " + this.name + " за: " + price);
                        currentPlayer.BuyProperty(this);
                        OnOwnerUpdated();
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
                    if (owner != null && owner != currentPlayer && !isMortgaged)
                    {
                        //Pay rent to somebody
                        //Calculate current rent
                        int rentToPay = CalculateRailRoadRent();
                        currentRent = rentToPay;
                        //Pay the rent to the owner
                        currentPlayer.PayRent(rentToPay, owner);

                        //Show a message about what happend

                    }
                    else if (owner == null)
                    {

                        //Show buy interface for the railroad
                        OnShowRailRoadBuyPanel.Invoke(this, currentPlayer);
                    }
                    else
                    {
                        //Is unowned and we cant afford it
                    }
                }

            break;

            case MonopolyNodeType.Tax:

                GameManager.instance.AddTaxToPool(price);
                currentPlayer.PayMoney(price);
                //Show a message about what happend
                OnUpdateMessage.Invoke(currentPlayer.name + " заплатил налог: " + price);

            break;

            case MonopolyNodeType.FreeParking:

                int tax = GameManager.instance.GetTaxPool();
                currentPlayer.CollectMoney(tax);
                //Show a message about what happend
                OnUpdateMessage.Invoke(currentPlayer.name + " получил возврат налогов: " + tax);

            break;

            case MonopolyNodeType.GoToJail:
                StartCoroutine(currentPlayer.GoToJail());
                //currentPlayer.GoToJailVoid();
                OnUpdateMessage.Invoke(currentPlayer.name + " должен отправиться в тюрьму");
                //continueTurn = false;

            break;

            case MonopolyNodeType.Chance:

                OnDrawChanceCard.Invoke(currentPlayer);
                continueTurn = false;

            break;

            case MonopolyNodeType.CommunityChest:

                OnDrawCommunityCard.Invoke(currentPlayer);
                continueTurn = false;

            break;

        }
        //Stop hear if needed
        if(!continueTurn)
        {
            return;
        }

        //Continue
        if (!playerIsHuman) 
        {
            Invoke("ContinueGame", GameManager.instance.SecondsBetwinTurns);
        }
        else 
        {
            bool canEndTurn = !GameManager.instance.RolledADouble && currentPlayer.ReadMoney >= 0;
            bool canRollDice = GameManager.instance.RolledADouble && currentPlayer.ReadMoney >= 0;
            //show UI
            OnShowHumanPanel.Invoke(true, canRollDice, canEndTurn);
        }
    }

    void ContinueGame()
    {
        //If the last roll was not a double 
        if(GameManager.instance.RolledADouble)
        {
            //roll agaid
            GameManager.instance.RollDice();
        }
        else
        {
            //not a double roll
            //switch player
            GameManager.instance.SwitchPlayer();
        }
    }

    int CalculatePropertyRent()
    {
        switch(numberOfHouses)
        {
            case 0:
                //Check if owner hs the full set of nodes
                var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(this);

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

    int CalculateUtilityRent()
    {
        int[] lastRolledDice = GameManager.instance.LastRolledDice;

        int result = 0;
        var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(this);
        if(allSame)
        {
            result = (lastRolledDice[0] + lastRolledDice[1]) * 10;
        }
        else
        {
            result += (lastRolledDice[0] + lastRolledDice[1]) * 4;
        }

        return result;
    }

    int CalculateRailRoadRent()
    {
        int result = 0;
        var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(this);

        int amount = 0;
        foreach (var item in list)
        {
            amount += (item.owner == this.owner) ? 1 : 0;
        }

        result = baseRent * (int)Mathf.Pow(2, amount - 1);


        return result;
    }

    void VisualizeHouses()
    {
        switch (numberOfHouses)
        {
            case 0:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 1:
                houses[0].SetActive(true);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 2:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 3:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(false);
                hotel.SetActive(false);
                break;

            case 4:
                houses[0].SetActive(true);
                houses[1].SetActive(true);
                houses[2].SetActive(true);
                houses[3].SetActive(true);
                hotel.SetActive(false);
                break;

            case 5:
                houses[0].SetActive(false);
                houses[1].SetActive(false);
                houses[2].SetActive(false);
                houses[3].SetActive(false);
                hotel.SetActive(true);
                break;
        }
    }

    public void BuildHouseOrHotel()
    {
        if(monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses++;
            VisualizeHouses();
        }
    }

    public int SellHouseOrHotel()
    {
        if (monopolyNodeType == MonopolyNodeType.Property && numberOfHouses > 0)
        {
            numberOfHouses--;
            VisualizeHouses();
            return houseCost / 2;
        }
        return 0;
    }

    public void ResetNode()
    {
        //If is morgtaged
        if (isMortgaged)
        {
            propertyImage.SetActive(true);
            mortgageImage.SetActive(false);
            isMortgaged = false;
        }
        //Reset houses and hotel
        if (monopolyNodeType == MonopolyNodeType.Property)
        {
            numberOfHouses = 0;
            VisualizeHouses();
        }
        //Reset the owner

        //Remove property from owner
        owner.RemoveProperty(this);
        owner.name = "";
        //Update UI
        OnOwnerUpdated();
    }
}
