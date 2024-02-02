using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommunityChest : MonoBehaviour
{

    [SerializeField] List<SCR_CummunityCard> cards = new List<SCR_CummunityCard>();
    [SerializeField] TMP_Text cardText;
    [SerializeField] GameObject cardHolderBackground;
    [SerializeField] float showTime = 3; //Hide card auto after 3 seconds
    [SerializeField] float moveDelay = 0.5f;//Maybe not used later

    List<SCR_CummunityCard> cardPool = new List<SCR_CummunityCard>();
    //Current card and current player
    SCR_CummunityCard pikedCard;
    Player currentPlayer;

    private void OnEnable()
    {
        MonopolyNode.OnDrawCommunityCard += DrawCard;
    }

    private void OnDisable()
    {
        MonopolyNode.OnDrawCommunityCard -= DrawCard;
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
        //Who is current player
        currentPlayer = cardTaker;

        //Show card
        cardHolderBackground.SetActive(true);

        //Fill in the text
        cardText.text = pikedCard.textOnCard;

        //Apply the effects based on the card we draw

    }

    public void ApplyCardEffect()//Close btn of the card
    {
        if(pikedCard.rewardMoney !=0 && !pikedCard.collectFromPlayers)
        {
            currentPlayer.CollectMoney(pikedCard.rewardMoney);
        }
        else if(pikedCard.penalityMoney != 0)
        {
            currentPlayer.PayMoney(pikedCard.penalityMoney);//Handle insuff sunds
        }
        else if(pikedCard.moveToBoardIndex != -1)
        {
            //Steps to goal
            int currentIndex = MonopolyBoard.Instance.route.IndexOf(currentPlayer.MyCurrentMonopolyNode);
            int lengthOfBoard = MonopolyBoard.Instance.route.Count;
            int stepsToMove = 0;
            if (currentIndex < pikedCard.moveToBoardIndex)
            {
                stepsToMove = pikedCard.moveToBoardIndex - currentIndex;
            }
            else if(currentIndex > pikedCard.moveToBoardIndex)
            {
                stepsToMove = lengthOfBoard - currentIndex + pikedCard.moveToBoardIndex;
            }
            //Start the move
            MonopolyBoard.Instance.MovePlayerToken(stepsToMove, currentPlayer);
        }
        else if(pikedCard.collectFromPlayers)
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
            
        }
        else if (pikedCard.goToJail)
        {
            currentPlayer.GoToJailVoid(MonopolyBoard.Instance.route.IndexOf(currentPlayer.MyCurrentMonopolyNode));
        }
        else if (pikedCard.jailFreeCard)
        {
            
        }
    }

}
