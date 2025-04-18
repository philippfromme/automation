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

public class ControlGroupsUI : MonoBehaviour
{
    private UIDocument uiDocument;
    private VisualElement controlGroups;

    public VisualTreeAsset controlGroupButtonTemplate;

    [SerializeField]
    private List<ControlGroupButton> controlGroupButtons;

    [SerializeField]
    private int selectedControlGroup = -1;

    private void OnEnable()
    {
        controlGroupButtons = new List<ControlGroupButton>();

        uiDocument = GetComponent<UIDocument>();

        controlGroups = uiDocument.rootVisualElement.Q<VisualElement>("ControlGroups");

        //AddControlGroup(1, () => Debug.Log("Control Group 1 clicked"));
        //AddControlGroup(6, () => Debug.Log("Control Group 6 clicked"));
        //AddControlGroup(3, () => Debug.Log("Control Group 2 clicked"));
        //AddControlGroup(2, () => Debug.Log("Control Group 3 clicked"));
        //AddControlGroup(4, () => Debug.Log("Control Group 4 clicked"));
        //AddControlGroup(5, () => Debug.Log("Control Group 5 clicked"));
        //AddControlGroup(5, () => Debug.Log("Control Group 5 clicked"));

        //SetSelectedControlGroup(2);
    }

    private void OnButtonClicked(ClickEvent evt, int index)
    {
        controlGroupButtons[index].onClick?.Invoke();
    }

    public void AddControlGroup(int controlGroupNumber, Action onClick)
    {
        var existing = controlGroupButtons.Find(x => x.controlGroupNumber == controlGroupNumber);

        if (existing != null)
            controlGroupButtons.Remove(existing);

        controlGroupButtons.Add(new ControlGroupButton
        {
            controlGroupNumber = controlGroupNumber,
            onClick = onClick
        });

        controlGroupButtons.Sort((a, b) => a.controlGroupNumber.CompareTo(b.controlGroupNumber));

        controlGroups.Clear();

        for (int i = 0; i < controlGroupButtons.Count; i++)
        {
            var button = controlGroupButtonTemplate.CloneTree();

            button.Q<Button>().text = controlGroupButtons[i].controlGroupNumber.ToString();
            button.Q<Button>().RegisterCallback<ClickEvent>(evt => OnButtonClicked(evt, i));

            controlGroups.Add(button);
        }
    }

    public void RemoveControlGroup(int controlGroupNumber)
    {
        for (int i = 0; i < controlGroups.childCount; i++)
        {
            if (controlGroupButtons[i].controlGroupNumber == controlGroupNumber)
            {
                controlGroupButtons.RemoveAt(i);

                controlGroups.RemoveAt(i);

                break;
            }
        }
    }

    public void SetSelectedControlGroup(int controlGroupNumber)
    {
        selectedControlGroup = controlGroupNumber;

        for (int i = 0; i < controlGroups.childCount; i++)
        {
            var button = controlGroups[i].Q<Button>();
            if (controlGroupButtons[i].controlGroupNumber == selectedControlGroup)
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
