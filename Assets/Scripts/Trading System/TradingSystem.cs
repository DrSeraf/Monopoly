using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Player;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class TradingSystem : MonoBehaviour
{
    public static TradingSystem instance;

    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject tradePanel;
    [SerializeField] GameObject resultPanel;
    [SerializeField] TMP_Text resultMessage;
    [Header("Left side")]
    [SerializeField]TMP_Text leftOffererNameText;
    [SerializeField] Transform leftCardGrid;
    [SerializeField] ToggleGroup leftToggleGroup;
    [SerializeField] TMP_Text leftYourMoneyText;
    [SerializeField] TMP_Text leftOfferMoney;
    [SerializeField] Slider leftMoneySlider;
    List<GameObject> leftCardPrefabList = new List<GameObject>();
    int leftChoosenMoneyAmount;
    MonopolyNode leftSelectedNode;
    Player leftPlayerReference;

    [Header("Middle side")]
    [SerializeField] Transform buttonGrid;
    [SerializeField] GameObject playerButtonPrefab;
    List<GameObject> playerButtonList = new List<GameObject>();

    [Header("Right side")]
    [SerializeField] TMP_Text rightOffererNameText;
    [SerializeField] Transform rightCardGrid;
    [SerializeField] ToggleGroup rightToggleGroup;
    [SerializeField] TMP_Text rightYourMoneyText;
    [SerializeField] TMP_Text rightOfferMoney;
    [SerializeField] Slider rightMoneySlider;
    List<GameObject> rightCardPrefabList = new List<GameObject>();
    int rightChoosenMoneyAmount;
    MonopolyNode rightSelectedNode;
    Player rightPlayerReference;

    [Header("Trade offer panel")]
    [SerializeField] GameObject tradeOfferPanel;
    [SerializeField] TMP_Text leftMessageText;
    [SerializeField] TMP_Text rightMessageText;
    [SerializeField] TMP_Text leftMoneyText;
    [SerializeField] TMP_Text rightMoneyText;
    [SerializeField] GameObject leftCard;
    [SerializeField] GameObject rightCard;
    [SerializeField] Image leftColorField;
    [SerializeField] Image rightColorField;
    //[SerializeField] Image leftPropertyImage;
    //[SerializeField] Image rightPropertyImage;



    //Message system
    public delegate void UpdateMessage(string message);
    public static UpdateMessage OnUpdateMessage;

    
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        tradePanel.SetActive(false);
        resultPanel.SetActive(false);
        tradeOfferPanel.SetActive(false);
    }
    //-------------------------------FIND MISSING PROPERTYS IN SET-------------------------AI

    public void FindMissingProperty(Player currentPlayer)
    {
        List<MonopolyNode> processedSet = null;
        MonopolyNode requestedNode = null;
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            //CHECK IF ALL HAVE BEEN PURCHASED
            bool notAllPurchased = list.Any(n => n.Owner == null);
            if (allSame || processedSet == list || notAllPurchased)
            {
                processedSet = nodeSet;
                continue;
            }
            //FIND THE OWNER BY OTHER PLAYER

            //BUT CHECK IF WE HAVE MORE THE THE AVERAGE
            if (list.Count == 2)
            {
                requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                if (requestedNode != null)
                {
                    //MAKE OFFER TO OWNER THE NODE
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }
            if (list.Count >= 3)
            {
                int hasMostOFSet = list.Count(n => n.Owner == currentPlayer);
                if (hasMostOFSet >= 2)
                {
                    requestedNode = list.Find(n => n.Owner != currentPlayer && n.Owner != null);
                    MakeTradeDecision(currentPlayer, requestedNode.Owner, requestedNode);
                    break;
                }
            }
        }
    }

    //-------------------------------MAKE TRADE DECISION-------------------------------------
    void MakeTradeDecision(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode)
    {
        //TRADE WITH MONEY IF POSSIBLE
        if (currentPlayer.ReadMoney >= CalculateValueOfNode(requestedNode))
        {
            //TRADE WITH MONEY ONLY 

            //MAKE TRADE OFFER
            MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, null, CalculateValueOfNode(requestedNode), 0);
            return;
        }

        //FIND ALL INCOMPLETE SET AND EXCLUDE THE SET WITCH THE REQUESTED NODE
        foreach (var node in currentPlayer.GetMonopolyNodes)
        {
            var checkedSet = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node).list;
            if (checkedSet.Contains(requestedNode))
            {
                //STOP CHECKING HERE
                continue;
            }
            //VALID NODE CHECK
            if (checkedSet.Count(n => n.Owner == currentPlayer) == 1)//VALID NODE CHECK
            {
                if (CalculateValueOfNode(node) + currentPlayer.ReadMoney >= requestedNode.price)
                {
                    int difference = CalculateValueOfNode(requestedNode) - CalculateValueOfNode(node);
                    //VALID TRADE POSSIBLE
                    if (difference > 0)
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, difference, 0);
                    }
                    else
                    {
                        MakeTradeOffer(currentPlayer, nodeOwner, requestedNode, node, 0, Mathf.Abs(difference));
                    }
                    //MAKE TRADE OFFER
                    break;
                }
            }
        }

        //FIND OUT IF ONLY ONE NODE OF THE FOUND SET IS OWNED

        //CALCULATE THE VALUE OF THAT NODE AND SEE IF WITH ENOUTH MONEY IT COULD BE AFFORDABLE

        //IF SO... MAKE TRADE OFFER
    }

    //-------------------------------MAKE TRADE OFFER----------------------------------------

    void MakeTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        if (nodeOwner.playerType == Player.PlayerType.AI)
        {
            ConsiderTradeOffer(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
        else if (nodeOwner.playerType == Player.PlayerType.Human)
        {
            //SHOW UI FOR HUMAN
            ShowTradeOfferPanel(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
        }
    }

    //-------------------------------CONCIDER TRADE OFFER----------------------------------AI
    void ConsiderTradeOffer(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        int valueOfTheTrade = (CalculateValueOfNode(requestedNode) + requestedMoney) - (CalculateValueOfNode(offeredNode) + offeredMoney);
        //SEL NODE FOR MONEY ONLY
        if (requestedNode == null && offeredNode != null && requestedMoney <= nodeOwner.ReadMoney/ 3 /*&& MonopolyBoard.Instance.PlayerHasAllNodesOfSet(requestedNode).allSame*/)
        {
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            TradeResult(true);
            return;
        }
        //JUST NORMAL TRADE
        if (valueOfTheTrade <= 0 /*&& MonopolyBoard.Instance.PlayerHasAllNodesOfSet(requestedNode).allSame*/)
        {
            //TRADE THE NODE IS VALID
            Trade(currentPlayer, nodeOwner, requestedNode, offeredNode, offeredMoney, requestedMoney);
            TradeResult(true);
        }
        else
        {
            TradeResult(false);
            //DEBUG LINE OR TELL PLAYER THAT REJECTED
            Debug.Log("AI rejected trage offer");
        }
    }
    //-------------------------------CALCULATE THE VALUE OF NODE---------------------------AI

    int CalculateValueOfNode(MonopolyNode requestedNode)
    {
        int value = 0;
        if (requestedNode != null)
        {
            if (requestedNode.monopolyNodeType == MonopolyNodeType.Property)
            {
                value = requestedNode.price + requestedNode.NumberOfHouses * requestedNode.houseCost;
            }
            else
            {
                value = requestedNode.price;
            }
            return value;
        }
        return value;
    }

    //-------------------------------TRADE THE NODE------------------------------------------
    void Trade(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        //CURRENT PLAYER NEEDS TO
        if (requestedNode != null)
        {
            currentPlayer.PayMoney(offeredMoney);
            requestedNode.ChangeOwner(currentPlayer);
            //NODE OWNER
            nodeOwner.CollectMoney(offeredMoney);
            nodeOwner.PayMoney(requestedMoney);
            if (offeredNode != null)
            {
                offeredNode.ChangeOwner(nodeOwner);
            }
            //SHOW A MESSAGE FOR THE UI
            string offeredNodeName = (offeredNode != null) ? " & " + offeredNode.name : "";
            OnUpdateMessage.Invoke(currentPlayer.name + " обменял " + requestedNode.name + " за " + offeredMoney +  offeredNodeName + " игроку " + nodeOwner.name);
        }
        else if (offeredNode != null && requestedNode == null)
        {
            currentPlayer.CollectMoney(requestedMoney);
            nodeOwner.PayMoney(requestedMoney);
            offeredNode.ChangeOwner(nodeOwner);
            //SHOW A MESSAGE FOR THE UI
            OnUpdateMessage.Invoke(currentPlayer.name + " продал " + offeredNode.name + " игроку " + nodeOwner.name + " за " + requestedMoney);
        }

        //HIDE UI FOR HUMAN
        CloseTradePanel();
    }

    //-------------------------------USER INTERFACE CONTETN-----------------------------HUMAN


    //-------------------------------CURRENT PLAYER-------------------------------------HUMAN

    void CreateLeftPanel()
    {
        leftOffererNameText.text = leftPlayerReference.name;
        List<MonopolyNode> referenceNodes = leftPlayerReference.GetMonopolyNodes;
        for (int i = 0; i < referenceNodes.Count; i++) 
        {
            GameObject tradeCard = Instantiate(cardPrefab, leftCardGrid, false);
            //SET UP TTHE ACTUAL CARD CONTENT
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i], leftToggleGroup);
            leftCardPrefabList.Add(tradeCard);
        }
        leftYourMoneyText.text = "Ваши деньги: " + leftPlayerReference.ReadMoney;
        //SET UP MONEY SLIDER

        leftMoneySlider.maxValue = leftPlayerReference.ReadMoney;
        leftMoneySlider.value = 0;
        UpdateLeftSlide(leftMoneySlider.value);

        //RESET OLD CONTENT


        tradePanel.SetActive(true);
    }

    public void UpdateLeftSlide(float value)
    {
        leftOfferMoney.text = "Деньги для сделки: " + leftMoneySlider.value.ToString();
    }
    public void UpdateRightSlide(float value)
    {
        rightOfferMoney.text = "Деньги для сделки: " + rightMoneySlider.value.ToString();
    }

    public void CloseTradePanel()
    {
        tradePanel.SetActive(false);
        ClearAll();
    }

    public void OpenTradePanel()
    {
        leftPlayerReference = GameManager.instance.GetCurrentPlayer;
        rightOffererNameText.text = "Выберите игрока";

        CreateLeftPanel();

        CreateMiddleButtons();
    }
    //-------------------------------SELECTED PLAYER------------------------------------HUMAN
    public void ShowRightPlayer(Player player)
    {
        rightPlayerReference = player;
        //RESET THE CURRENT CONTENT
        ClearRightPanel();
        //SHOW RIGHT PLAYER OF ABOVE PLAYER
        rightOffererNameText.text = rightPlayerReference.name;
        List<MonopolyNode> referenceNodes = rightPlayerReference.GetMonopolyNodes;
        for (int i = 0; i < referenceNodes.Count; i++)
        {
            GameObject tradeCard = Instantiate(cardPrefab, rightCardGrid, false);
            //SET UP TTHE ACTUAL CARD CONTENT
            tradeCard.GetComponent<TradePropertyCard>().SetTradeCard(referenceNodes[i], rightToggleGroup);
            rightCardPrefabList.Add(tradeCard);
        }
        rightYourMoneyText.text = "Деньги игрока: " + rightPlayerReference.ReadMoney;
        //SET UP MONEY SLIDER

        rightMoneySlider.maxValue = rightPlayerReference.ReadMoney;
        rightMoneySlider.value = 0;
        UpdateLeftSlide(rightMoneySlider.value);
        //UPDATE THE MONEY AND THE SLIDER
    }

    //SET UP MIDDLE
    void CreateMiddleButtons()
    {
        //CLEAR CONTENT
        for(int i = playerButtonList.Count-1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //LOOP THROGH ALL PLAYER

        List<Player> allPlayer = new List<Player>();
        allPlayer.AddRange(GameManager.instance.GetPlayers);
        allPlayer.Remove(leftPlayerReference);

        //AND THE BUTTONS FOR THEM
        foreach(var player in allPlayer)
        {
            GameObject newPlayerButton = Instantiate(playerButtonPrefab,buttonGrid,false);
            newPlayerButton.GetComponent<TradePlayerButton>().SetPlayer(player);


            playerButtonList.Add(newPlayerButton);
        }
    }


    void ClearAll()//IF WE OPEN OR CLOSE TRADE SYSTEM
    {
        rightOffererNameText.text = "Выберите игрока";
        rightYourMoneyText.text = "Деньги игрока: 0";
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlide(rightMoneySlider.value);
        //CLEAR MIDDEL BUTTONS
        for (int i = playerButtonList.Count - 1; i >= 0; i--)
        {
            Destroy(playerButtonList[i]);
        }
        playerButtonList.Clear();

        //CLEAR LEFT CARD CONTENT
        for (int i = leftCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(leftCardPrefabList[i]);
        }
        leftCardPrefabList.Clear();

        //CLEAR  RIGHT CARD CONTENT
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();
    }

    void ClearRightPanel()
    {
        for (int i = rightCardPrefabList.Count - 1; i >= 0; i--)
        {
            Destroy(rightCardPrefabList[i]);
        }
        rightCardPrefabList.Clear();
        //RESET THE SLIDER
        rightMoneySlider.maxValue = 0;
        rightMoneySlider.value = 0;
        UpdateRightSlide(rightMoneySlider.value);
    }

    //-------------------------------MAKE OFFER-----------------------------------------HUMAN

    public void MakeOfferButton()//HUMAN INPUT OFFER
    {
        MonopolyNode requestedNode = null;
        MonopolyNode offeredNode = null;
        if (rightPlayerReference == null)
        {
            //ERROR MESSAGE
            return;
        }
        //LEFT SELECTED NODE
        Toggle offeredToggle = leftToggleGroup.ActiveToggles().FirstOrDefault();
        if(offeredToggle != null)
        {
            offeredNode = offeredToggle.GetComponentInParent<TradePropertyCard>().Node();
        }
        //RIGH SELECTED NODE
        Toggle requestedToggle = rightToggleGroup.ActiveToggles().FirstOrDefault();
        if (requestedToggle != null)
        {
            requestedNode = requestedToggle.GetComponentInParent<TradePropertyCard>().Node();
        }

        MakeTradeOffer(leftPlayerReference, rightPlayerReference, requestedNode, offeredNode, (int)leftMoneySlider.value, (int)rightMoneySlider.value);
    }

    //-------------------------------TRADE RESULT---------------------------------------HUMAN

    void TradeResult(bool accepted)
    {
        if (accepted) 
        {
            resultMessage.text = rightPlayerReference.name + " <color=green>принял</color> " + "обмен.";
        }
        else 
        {
            resultMessage.text = rightPlayerReference.name + " <color=red>отказал</color> " + "в обмене.";
        }

        resultPanel.SetActive(true);
    }

    //-------------------------------TRADE OFFER PANEL----------------------------------HUMAN

    void ShowTradeOfferPanel(Player currentPlayer, Player nodeOwner, MonopolyNode requestedNode, MonopolyNode offeredNode, int offeredMoney, int requestedMoney)
    {
        tradeOfferPanel.SetActive(true);
        leftMessageText.text = currentPlayer.name + "предлагает:";
        rightMessageText.text = "За " + nodeOwner.name + "имущество: ";
        leftMoneyText.text = "+ М" + offeredMoney;
        rightMoneyText.text = "+ М" + requestedMoney;
        leftCard.SetActive(offeredNode != null ? true : false);
        rightCard.SetActive(requestedNode != null ? true : false);

        if (leftCard.activeInHierarchy)
        {
            leftColorField.color = (offeredNode.propertyColorField != null) ? offeredNode.propertyColorField.color : Color.white;
        }

        if (rightCard.activeInHierarchy)
        {
            rightColorField.color = (requestedNode.propertyColorField != null) ? offeredNode.propertyColorField.color : Color.white;
        }
    }

    public void AcceptOffer()
    {

    }

    public void RejectOffer()
    {

    }

    //-------------------------------
}
