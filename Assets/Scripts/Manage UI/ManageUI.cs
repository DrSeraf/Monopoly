using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageUI : MonoBehaviour
{

    public static ManageUI instance;

    [SerializeField] GameObject managePanel;//To show and hide
    [SerializeField] Transform propertyGrid;//To parent property sets to it
    [SerializeField] GameObject propertySetPrefab;
    Player playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();
    [SerializeField] TMP_Text yourMoneyText;
    [SerializeField] TMP_Text systemMessageText;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        managePanel.SetActive(false);
    }

    public void OpenManager()//Call from manage button
    {
        playerReference = GameManager.instance.GetCurrentPlayer;
        //Get all nodes as node sets
        List<MonopolyNode> processedSet = null;
        foreach (var node in playerReference.GetMonopolyNodes)
        {
            var (list, allSame) = MonopolyBoard.Instance.PlayerHasAllNodesOfSet(node);
            List<MonopolyNode> nodeSet = new List<MonopolyNode>();
            nodeSet.AddRange(list);
            if (nodeSet != null && list != processedSet)
            {
                //Update processed first
                processedSet = list;
                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //Create prefabs witch all nodes owned by the player
                GameObject newPropertySet = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertySet.GetComponent<ManagePropertyUI>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
        managePanel.SetActive(true);
        UpdateMoneyText();
    }

    public void CloseManager()
    {
        managePanel.SetActive(false);
        for (int i = propertyPrefabs.Count-1; i >= 0; i--)
        {
            Destroy(propertyPrefabs[i]);
        }
        propertyPrefabs.Clear();
    }
    public void UpdateMoneyText()
    {
        string showMoney = (playerReference.ReadMoney > 0) ? "<color=green>М" + playerReference.ReadMoney : "<color=red>М" + playerReference.ReadMoney;
        yourMoneyText.text = "Ваши деньги: " + showMoney;
    }

    public void UpdateSystemMessage(string message)
    {
        systemMessageText.text = message;
        StartCoroutine(RemoveMessageAfterDelay(3f));
    }
    IEnumerator RemoveMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Ждем указанное количество секунд
        systemMessageText.text = ""; // Очищаем текст сообщения
    }
}
