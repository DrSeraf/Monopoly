using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowPanel : MonoBehaviour
{

    [SerializeField] GameObject humanPanel;
    [SerializeField] Button rollDiseButton;
    [SerializeField] Button endTurnButton;
    [SerializeField] Button jailFreeCard1;
    [SerializeField] Button jailFreeCard2;

    private void OnEnable()
    {
        GameManager.OnShowHumanPanel += ShowPanel;
        MonopolyNode.OnShowHumanPanel += ShowPanel;
        CommunityChest.OnShowHumanPanel += ShowPanel;
        ChanceField.OnShowHumanPanel += ShowPanel;
        Player.OnShowHumanPanel += ShowPanel;
    }

    private void OnDisable()
    {
        GameManager.OnShowHumanPanel -= ShowPanel;
        MonopolyNode.OnShowHumanPanel -= ShowPanel;
        CommunityChest.OnShowHumanPanel -= ShowPanel;
        ChanceField.OnShowHumanPanel -= ShowPanel;
        Player.OnShowHumanPanel -= ShowPanel;
    }

    void ShowPanel(bool showPanel, bool enableRollDice, bool enableEndTurd, bool hasChanceJailCard, bool hasCommunityJailCard)
    {
        humanPanel.SetActive(showPanel);
        rollDiseButton.interactable = enableRollDice;
        endTurnButton.interactable = enableEndTurd;
        jailFreeCard1.interactable = hasChanceJailCard;
        jailFreeCard2.interactable = hasCommunityJailCard;
    }

}
