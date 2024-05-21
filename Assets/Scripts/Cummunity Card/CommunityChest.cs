using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CommunityChest : MonoBehaviour
{
    public static CommunityChest instance;
    [SerializeField] List<SCR_CummunityCard> cards = new List<SCR_CummunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3; //Hide card auto after 3 seconds
    [SerializeField] Button closeCardButton;

    List<SCR_CummunityCard> cardPool = new List<SCR_CummunityCard>();
    List<SCR_CummunityCard> usedCardPool = new List<SCR_CummunityCard>();
    //Current card and current player
    SCR_CummunityCard jailFreeCard;
    SCR_CummunityCard pikedCard;
    Player currentPlayer;

    //Human input panel 
    public delegate void ShowHumanPanel(bool activatePanel, bool activateRollDice, bool activateEndTurn, bool hasChanceJailCard, bool hasCommunityJailCard);
    public static ShowHumanPanel OnShowHumanPanel;

    private void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += DrawCard;
    }

    private void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= DrawCard;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        cardHolderBackground.SetActive(false);
        //Add all cards to the pool


        cardPool.AddRange(cards);

        //Shuffle the cards
        ShuffleCards();
    }

    void ShuffleCards()
    {
        for (int i = 0; i < cardPool.Count; i++)
        {
            int index = Random.Range(0, cardPool.Count);
            SCR_CummunityCard tempCard = cardPool[index];
            cardPool[index] = cardPool[i];
            cardPool[i] = tempCard;
        }
    }

    void DrawCard(Player cardTaker)
    {

        //Draw an actual card
        pikedCard = cardPool[0];
        cardPool.RemoveAt(0);


        if (pikedCard.jailFreeCard)
        {
            jailFreeCard = pikedCard;
        }
        else
        {
            usedCardPool.Add(pikedCard);
        }


        if (cardPool.Count == 0)
        {
            //Put back all cards
            cardPool.AddRange(usedCardPool);
            usedCardPool.Clear();

            //Shuffle all
            ShuffleCards();
        }
        //Who is current player
        currentPlayer = cardTaker;

        //Show card
        cardHolderBackground.SetActive(true);

        //Fill in the text
        cardText.text = pikedCard.textOnCard;

        //Deactivate button if we are an AI player
        if(currentPlayer.playerType == Player.PlayerType.AI)
        {
            closeCardButton.interactable = false;
            Invoke("ApplyCardEffect", showTime);
        }
        else
        {
            closeCardButton.interactable= true;
        }

    }

    public void ApplyCardEffect()//Close btn of the card
    {
        bool isMoving = false;
        if (pikedCard.rewardMoney != 0 && !pikedCard.collectFromPlayers)
        {
            currentPlayer.CollectMoney(pikedCard.rewardMoney);
        }
        else if (pikedCard.penalityMoney != 0)
        {
            currentPlayer.PayMoney(pikedCard.penalityMoney);//Handle insuff sunds
        }
        else if (pikedCard.moveToBoardIndex != -1)
        {
            isMoving = true;
            //Steps to goal
            int currentIndex = MonopolyBoard.Instance.route.IndexOf(currentPlayer.MyCurrentMonopolyNode);
            int lengthOfBoard = MonopolyBoard.Instance.route.Count;
            int stepsToMove = 0;
            if (currentIndex < pikedCard.moveToBoardIndex)
            {
                stepsToMove = pikedCard.moveToBoardIndex - currentIndex;
            }
            else if (currentIndex > pikedCard.moveToBoardIndex)
            {
                stepsToMove = lengthOfBoard - currentIndex + pikedCard.moveToBoardIndex;
            }
            //Start the move
            MonopolyBoard.Instance.MovePlayerToken(stepsToMove, currentPlayer);
        }
        else if (pikedCard.collectFromPlayers)
        {
            int totalCollected = 0;
            List<Player> allPlayers = GameManager.instance.GetPlayers;

            foreach (var player in allPlayers)
            {
                if (player != currentPlayer)
                {
                    //Prevent bunkrupcy
                    int amount = Mathf.Min(player.ReadMoney, pikedCard.rewardMoney);
                    player.PayMoney(amount);
                    totalCollected += amount;
                }
            }
            currentPlayer.CollectMoney(totalCollected);
        }
        else if (pikedCard.streetRepairs)
        {
            int[] allBuildings = currentPlayer.CountHousesAndHotels();
            int totalCosts = pikedCard.streetRepairsHousPrice * allBuildings[0] + pikedCard.streetRepairsHotelPrice * allBuildings[1];
            currentPlayer.PayMoney(totalCosts);
        }
        else if (pikedCard.goToJail)   
        {   isMoving = true;
            currentPlayer.GoToJailVoid(MonopolyBoard.Instance.route.IndexOf(currentPlayer.MyCurrentMonopolyNode));
        }
        else if (pikedCard.jailFreeCard)//Jail free card
        {
            currentPlayer.AddCommunityJailFreeCard();
        }
        cardHolderBackground.SetActive(false);
        ContinueGame(isMoving);
    }

    void ContinueGame(bool isMoving)
    {
        if(currentPlayer.playerType == Player.PlayerType.AI)
        {
            if(!isMoving)
            {
                GameManager.instance.Continue();
            }
        }
        else//Human inputs
        {
            if (!isMoving)
            {
                bool jail1 = currentPlayer.HasChanceJailFreeCard;
                bool jail2 = currentPlayer.HasCommunityJailFreeCard;
                OnShowHumanPanel.Invoke(true, GameManager.instance.RolledADouble, !GameManager.instance.RolledADouble, jail1, jail2);
            } 
        }
    }

    public void AddBackJailFreeCard()
    {
        usedCardPool.Add(jailFreeCard);
        jailFreeCard = null;
    }

}
