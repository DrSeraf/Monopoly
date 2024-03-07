using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManageUI : MonoBehaviour
{

    [SerializeField] GameObject managePanel;//To show and hide
    [SerializeField] Transform propertyGrid;//To parent property sets to it
    [SerializeField] GameObject propertySetPrefab;
    Player playerReference;
    List<GameObject> propertyPrefabs = new List<GameObject>();

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
            List<MonopolyNode> nodeSet = list;
            if (nodeSet != null && nodeSet != processedSet)
            {
                //Update processed first
                processedSet = nodeSet;
                nodeSet.RemoveAll(n => n.Owner != playerReference);

                //Create prefabs witch all nodes owned by the player
                GameObject newPropertySet = Instantiate(propertySetPrefab, propertyGrid, false);
                newPropertySet.GetComponent<ManagePropertyUI>().SetProperty(nodeSet, playerReference);

                propertyPrefabs.Add(newPropertySet);
            }
        }
        managePanel.SetActive(true);
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

}
