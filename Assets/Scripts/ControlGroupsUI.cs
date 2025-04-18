using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlGroupButton
{
    public int controlGroupNumber;
    public Action onClick;
}

public class ControlGroupButtonClickedEventArgs : EventArgs
{
    public int controlGroupNumber;
    public ControlGroupButtonClickedEventArgs(int controlGroupNumber)
    {
        this.controlGroupNumber = controlGroupNumber;
    }

}

public class ControlGroupsUI : MonoBehaviour
{
    private VisualElement controlGroupsElement;

    public VisualTreeAsset controlGroupButtonTemplate;

    [SerializeField]
    private List<ControlGroupButton> controlGroupButtons;

    public EventHandler<ControlGroupButtonClickedEventArgs> controlGroupButtonClicked;

    private void OnEnable()
    {
        controlGroupButtons = new List<ControlGroupButton>();

        controlGroupsElement = GetComponent<UIDocument>().rootVisualElement.Q<VisualElement>("ControlGroups");

        GameManager gameManager = FindObjectOfType<GameManager>();

        gameManager.ControlGroups.ControlGroupsChanged += (sender, e) =>
        {
            Debug.Log("Control groups have changed!");

            Rerender();
        };

        gameManager.SelectedControlGroupChanged += (sender, e) =>
        {
            Debug.Log("Selected control group has changed: " + gameManager.SelectedControlGroup);

            SetSelectedControlGroup(gameManager.SelectedControlGroup);
        };
    }

    private void OnButtonClicked(ClickEvent evt, int index)
    {
        controlGroupButtons[index].onClick?.Invoke();
    }

    public void Rerender()
    {
        controlGroupsElement.Clear();

        GameManager gameManager = FindObjectOfType<GameManager>();

        ControlGroups controlGroups = gameManager.ControlGroups;

        foreach (var group in controlGroups.GetControlGroups())
        {
            ControlGroupButton controlGroupButton = new ControlGroupButton
            {
                controlGroupNumber = group.Key,
                onClick = () =>
                {
                    gameManager.SelectControlGroup(group.Key);
                }
            };

            controlGroupButtons.Add(controlGroupButton);
        }
    }

    public void SetSelectedControlGroup(int controlGroupNumber)
    {
        for (int i = 0; i < controlGroupsElement.childCount; i++)
        {
            var button = controlGroupsElement[i].Q<Button>();

            if (controlGroupButtons[i].controlGroupNumber == controlGroupNumber)
            {
                button.AddToClassList("selected");
            }
            else
            {
                button.RemoveFromClassList("selected");
            }
        }
    }
}
