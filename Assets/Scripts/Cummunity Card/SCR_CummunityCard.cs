using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Community Card", menuName = "Monopoly/Cards/Community")]
public class SCR_CummunityCard : ScriptableObject
{

    public string textOnCard;// Description
    public int rewardMoney;// Get money
    public int penalityMoney;// Pay money
    public int moveToBoardIndex = -1;
    public bool collectFromPlayers;
    public bool streetRepairs;
    public bool goToJail;
    public bool jailFreeCard;

}
