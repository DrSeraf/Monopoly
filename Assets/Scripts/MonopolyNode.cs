// ������������ ������������ ����
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Properties;

// ������������ ��� ����� ����� � ���� ���������
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

// ����� MonopolyNode, ������� ������������ ����� ���� � ���� ���������
public class MonopolyNode : MonoBehaviour
{
    // ��� ����
    public MonopolyNodeType monopolyNodeType;

    // ��� ����
    [Header("Property Name")]
    [SerializeField] internal new string name;
    [SerializeField] TMP_Text nameText;

    // ���� ����
    [Header("Property Price")]
    public int price;
    [SerializeField] TMP_Text priceText;

    // �������� ����� ����
    [Header("Property Rent")]
    [SerializeField] bool calculateRentAuto;
    [SerializeField] int currentRent;
    [SerializeField] internal int baseRent;
    [SerializeField] internal int[] rentWithHouse;

    // ����� ����
    [Header("Property Mortgage")]
    [SerializeField] GameObject mortgageImage;
    [SerializeField] GameObject propertyImage;
    [SerializeField] bool isMortgaged;
    [SerializeField] int mortgageValue;

    // �������� ����
    [Header("Property Owner")]
    [SerializeField] GameObject ownerBar;
    [SerializeField] TMP_Text ownerText;

    // �����, ������� ���������� ��� ��������� �������� � ���������� Unity
    private void OnValidate()
    {
        // ���������� ������ �����
        if (nameText != null)
        {
            nameText.text = name;
        }

        // ������ ���� � �������� �����
        if (calculateRentAuto)
        {
            // ������ ���� � �������� ����� ��� ���� Property
            if (monopolyNodeType == MonopolyNodeType.Property)
            {
                if(baseRent > 0)
                {
                    price = 3 * (baseRent * 10);

                    // ������ ��������� ������
                    mortgageValue = price/2;

                    // ������ �������� ����� � �����
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

            // ������ ��������� ������ ��� ����� Utility � RailRoad
            if (monopolyNodeType == MonopolyNodeType.Utility)
            {
                mortgageValue = price / 2;
            }

            if (monopolyNodeType == MonopolyNodeType.RailRoad)
            {
                mortgageValue = price / 2;
            }
        }

        // ���������� ������ ����
        if (priceText != null)
        {
            priceText.text = "$ " + price;
        }
        //Update the owner
    }

    // ����� ��� ������ ��������
    public int MortgageProperty()
    {
        isMortgaged = true;
        mortgageImage.SetActive(true);
        propertyImage.SetActive(false);
        return mortgageValue;
    }

    // ����� ��� ������������ ������ � ��������
    public void UnMortgageProperty()
    {
        isMortgaged = false;
        mortgageImage.SetActive(false);
        propertyImage.SetActive(true);
    }

    // ����� ��� ���������� ���������
    public bool IsMortgaged => isMortgaged;
    public int MortgageValue => mortgageValue;

    //Update owner
    public void OnOwnerUpdated()
    {
        if(ownerBar != null)
        {
            if(ownerText.text != "")
            {
                ownerBar.SetActive(true);
                //ownerText.text = owner.name;
            }
            else
            {
                ownerBar.SetActive(false);
                ownerText.text = "";
            }
        }
    }
}
