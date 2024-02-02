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
    [SerializeField] float secondsBetwinTurns = 3;

    [Header("Player Info")]
    [SerializeField] GameObject playerInfoPrefab;
    [SerializeField] Transform playerPanel;//For the palyer info prefabs to become parentet to
    [SerializeField] List<GameObject> playerTokenList = new List<GameObject>();

    //about the rolling dice
    int[] rolledDice;
    bool rolledADouble;
    public bool RolledADouble => rolledADouble;
    public void RestRolledADouble() => rolledADouble = false;
    int doubleRollCount;
    //Tax pool
    int taxPool = 0;

    //Pass over goal to get money
    public int GetGoMoney => goMoney;
    public float SecondsBetwinTurns => secondsBetwinTurns;
    public List<Player> GetPlayers => playerList;

    //Message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    //Debug
    public bool alwaysDoubleRoll = false;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Initialize();
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            RollDice();
        }
        else
        {
            //show UI for human inputs
        }
        
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
        bool allowedToMove = true;
        //Reset last roll
        rolledDice = new int[2];
        //Any roll dice and store them
        rolledDice[0] = Random.Range(1,7);
        rolledDice[1] = Random.Range(1, 7);
        Debug.Log("Rolled dice are: " + rolledDice[0] + " & " + rolledDice[1]);

        //Debug
        if(alwaysDoubleRoll)
        {
            rolledDice[0] = 1;
            rolledDice[1] = 1;
        }



        //Check for double 
        rolledADouble = rolledDice[0] == rolledDice[1];
        //throw 3 times in a row -> jail end turn 

        //is in jail already 
        if (playerList[currentPlayer].IsInJail)
        {
            playerList[currentPlayer].IncreaseNumTurnInJail();

            if(rolledADouble)
            {
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=green>����� ����� �� ������</color>, ������ ��� ����� �����");
                doubleRollCount++;
                //Move the player
            }
            else if (playerList[currentPlayer].NumberTurnsInJail >= maxTurnsInJail)
            {
                //We have been long enough here
                playerList[currentPlayer].SetOutOfJail();
                OnUpdateMessage.Invoke(playerList[currentPlayer].name + " <color=green>����� ����� �� ������</color>");
                //Allowed to leave
            }
            else
            {
                allowedToMove = false;
            }
        }
        else//not in jail
        {
            //Reset Double rolles
            if(!rolledADouble)
            {
                doubleRollCount = 0;
            }
            else
            {
                doubleRollCount++;
                if(doubleRollCount >= 3)
                {
                    //Move to jail
                    int indexOnBoard = MonopolyBoard.Instance.route.IndexOf(playerList[currentPlayer].MyCurrentMonopolyNode);
                    playerList[currentPlayer].GoToJailVoid(indexOnBoard);
                    doubleRollCount = 0;
                    rolledADouble = false;
                    OnUpdateMessage.Invoke(playerList[currentPlayer].name + " 3 ���� �������� ����� � ����� � <color=red>������!</color>");
                    return;
                }
            }
        }

        //can we leave jail

        //move anyhow if allowed
        if(allowedToMove == true)
        {
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " ������ " + rolledDice[0] + " � " + rolledDice[1]);
            StartCoroutine(DelayBeforeMove(rolledDice[0] + rolledDice[1]));
            //show or hide UI
        }
        else
        {
            //Switch player
            OnUpdateMessage.Invoke(playerList[currentPlayer].name + " ������ ���������� � ������. ����� ������: " + playerList[currentPlayer].NumberTurnsInJail);
            StartCoroutine(DelayBetwinSwitchPlayer());
        }
    }

    IEnumerator DelayBeforeMove(int rolledDice)
    {
        yield return new WaitForSeconds(secondsBetwinTurns);
        //if we are allowed to move do so
        gameBoard.MovePlayerToken(rolledDice, playerList[currentPlayer]);
        //else we switch
    }

    IEnumerator DelayBetwinSwitchPlayer()
    {
        yield return new WaitForSeconds(secondsBetwinTurns);
        SwitchPlayer();
    }

    public void SwitchPlayer()
    {
        currentPlayer++;
        //Rolled double?
        doubleRollCount = 0;

        //Overflow check
        if (currentPlayer >= playerList.Count) 
        {
            currentPlayer = 0;
        }

        //check if in jail

        //is player AI
        if (playerList[currentPlayer].playerType == Player.PlayerType.AI)
        {
            RollDice();
        }


        //if human - show UI

    }

    public int[] LastRolledDice => rolledDice;

    public void AddTaxToPool(int amount)
    {
        taxPool += amount;
    }

    public int GetTaxPool()
    {
        //Temp store tax pool
        int currentTaxCollected = taxPool;
        //Reset the pool
        taxPool = 0;
        //Send temp tax
        return currentTaxCollected;

    }
}
