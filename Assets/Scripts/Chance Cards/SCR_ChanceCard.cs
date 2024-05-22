using System.Collections;
using System.Collections.Generic;
//using UnityEditor.XR;
using UnityEngine;

[CreateAssetMenu(fileName = "New Chance Card", menuName = "Monopoly/Cards/Chance")]
public class SCR_ChanceCard : ScriptableObject
{

    public string textOnCard;// Description
    public int rewardMoney;// Get money
    public int penalityMoney;// Pay money
    public int moveToBoardIndex = -1;
    public bool payToPlayers;

    [Header("Move To Locations")]
    public bool nextRailRoad;
    public bool nextutility;
    public int moveStepsBackwards;

    [Header("Jail Content")]
    public bool goToJail;
    public bool jailFreeCard;

    [Header("Street Repairs")]
    public bool streetRepairs;
    public int streetRepairsHousPrice = 25;
    public int streetRepairsHotelPrice = 100;

}
