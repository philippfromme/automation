using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private List<Unit> _units;

    private List<Unit> _selectedUnits;

    private LayerMask _groundLayer;

    public GameObject DestinationSprite;

    private ControlGroups _controlGroups = new ControlGroups(Debug.unityLogger);

    private const float boxSelectionThreshold = 10f;

    public bool isShiftDown = false;

    private void Start()
    {
        _units = new List<Unit>();
        _selectedUnits = new List<Unit>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isShiftDown = true;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isShiftDown = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 destination = hit.point;

                Debug.Log("Destination: " + destination);

                foreach (Unit unit in _selectedUnits)
                {
                    unit.MoveTo(destination);
                }

                DestinationSprite.transform.position = destination;
            }
        }

        if (Input.GetMouseButtonDown(0)) {
            BoxSelection.Instance.mouseButtonWentDown();
        }

        if (Input.GetMouseButton(0)) {
            BoxSelection.Instance.MouseButtonIsDown();
        }

        if (Input.GetMouseButtonUp(0)) {
            BoxSelection.Instance.mouseButtonWentUp();
        }

        // Control groups
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyUp(i.ToString()))
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    AddControlGroup(i);
                }
                else
                {
                    SelectControlGroup(i);
                }
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        _units.Add(unit);
    }

    public void SelectUnit(Unit unit, bool addToSelection)
    {
        if (addToSelection && !_selectedUnits.Contains(unit))
        {
            _selectedUnits.Add(unit);
        } else
        {
            DeselectAllUnits();

            _selectedUnits.Clear();

            _selectedUnits.Add(unit);
        }

        unit.Select();
    }

    private void DeselectAllUnits()
    {
        for (int i = 0; i < _selectedUnits.Count; i++)
        {
            _selectedUnits[i].Deselect();
        }

        _selectedUnits.Clear();
    }

    public void BoxSelect(Rect selectionBox)
    {

        Debug.Log("BoxSelect called");

        if (selectionBox.xMax > selectionBox.xMin + boxSelectionThreshold && selectionBox.yMax > selectionBox.yMin + boxSelectionThreshold) {
            if (!isShiftDown) {
                DeselectAllUnits();

                _selectedUnits.Clear();
            }

            for (int i = 0; i < _units.Count; i++)
            {
                Debug.Log("Checking unit: " + _units[i].name);

                if (selectionBox.Contains(Camera.main.WorldToScreenPoint(_units[i].transform.position))) {
                    Debug.Log("Unit " + _units[i].name + " is within selection box.");

                    SelectUnit(_units[i], true);
                } else {
                    Debug.Log("Unit " + _units[i].name + " is outside selection box.");
                }
            }
        } else
        {
            DeselectAllUnits();
        }
    }

    private void AddControlGroup(int keyNumber)
    {
        _controlGroups.AddGroup(keyNumber, _selectedUnits.Select(n => n.selectableObject).ToList());

        ControlGroupsUI controlGroupsUI = FindObjectOfType<ControlGroupsUI>();

        if (controlGroupsUI != null)
        {
            controlGroupsUI.AddControlGroup(keyNumber, () => SelectControlGroup(keyNumber));

            controlGroupsUI.SetSelectedControlGroup(keyNumber);

            for (int i = 0; i < 10; i++)
            {
                Debug.Log("Checking control group " + i + ": " + _controlGroups.GetGroup(i)?.Count);

                if (_controlGroups.GetGroup(keyNumber) == null || _controlGroups.GetGroup(keyNumber).Count == 0)
                {
                    Debug.Log("Control group " + keyNumber + " is empty, removing UI.");

                    controlGroupsUI.RemoveControlGroup(keyNumber);
                    break;
                }
            }
        }
    }

    private void SelectControlGroup(int keyNumber)
    {
        DeselectAllUnits();

        if (_controlGroups.GetGroup(keyNumber) == null || _controlGroups.GetGroup(keyNumber).Count == 0)
        {
            Debug.Log("Control group " + keyNumber + " is empty.");

            return;
        }

        foreach (var selectableObject in _controlGroups.GetGroup(keyNumber))
        {
            SelectUnit(selectableObject.unit, true);
        }

        ControlGroupsUI controlGroupsUI = FindObjectOfType<ControlGroupsUI>();

        if (controlGroupsUI != null)
        {
            controlGroupsUI.SetSelectedControlGroup(keyNumber);
        }
    }
}
