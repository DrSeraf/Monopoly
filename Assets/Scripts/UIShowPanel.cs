using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShowPanel : MonoBehaviour
{

    [SerializeField] GameObject humanPanel;
    [SerializeField] Button rollDiseButton;
    [SerializeField] Button endTurnButton;

    private void OnEnable()
    {
        GameManager.OnShowHumanPanel += ShowPanel;
    }

    private void OnDisable()
    {
        GameManager.OnShowHumanPanel -= ShowPanel;
    }

    void ShowPanel(bool showPanel, bool enableRollDice, bool enableEndTurd)
    {
        humanPanel.SetActive(showPanel);
        rollDiseButton.interactable = enableRollDice;
        endTurnButton.interactable = enableEndTurd;
    }

}
