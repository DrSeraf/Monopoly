using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] MonopolyBoard gameBoard;
    [SerializeField] List<Player> playerList = new List<Player>();
    [SerializeField] int currentPlayer;

    [Header("Global Game Settings")]
    [SerializeField] int maxTurnsInJail = 3; //Setting for how long in jail 
    [SerializeField] int startMoney = 1500;
    [SerializeField] int goMoney = 500;

    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel;//For the palyer info prefabs to become parentet to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    //about the rolling dice
    int[] rolledDice;
    bool rolledADouble;
    int doubleRollCount;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        //Create all players
        for (int i = 0; i < playerList.Count; i++)
        {
            GameObject infoObject = Instantiate(playerInfoPrefab, playerPanel, false);
            PlayerInfo info = infoObject.GetComponent<PlayerInfo>();

            //Random token
            int randomIndex = Random.Range(0, playerTokenList.Count);
            //Instatiate
            GameObject newToken = Instantiate(playerTokenList[randomIndex], gameBoard.route[0].transform.position, Quaternion.identity);

            playerList[i].Inititialize(gameBoard.route[0], startMoney, info, newToken);
        }
    }

    public void RollDice()//Press button from human - or auto from ai
    {
        //Reset last roll
        rolledDice = new int[2];
        //Any roll dice and store them
        rolledDice[0] = Random.Range(1,7);
        rolledDice[1] = Random.Range(1, 7);
        //Check for double 
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail end turn 

        //is in jail already 

        //can we leave jail

        //move anyhove if allowed

        //show or hide UI
    }

    IEnumerator DelayBeforeMove()
    {
        yield return new WaitForSeconds(2f);
        //if we are allowed to move do so

        //else we switch
    }

}
