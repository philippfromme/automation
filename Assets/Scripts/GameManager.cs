using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    private List<Unit> _units;

    private List<Unit> _selectedUnits;

    private LayerMask _groundLayer;

    public GameObject DestinationSprite;

    public ControlGroups ControlGroups = new ControlGroups(Debug.unityLogger);

    public int SelectedControlGroup = -1;

    public event EventHandler SelectedControlGroupChanged;

    public bool isShiftDown = false;
    public bool isControlDown = false;

    private void Start()
    {
        _units = new List<Unit>();
        _selectedUnits = new List<Unit>();
    }

    public void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
    }

    private void HandleMouseInput()
    {
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

        if (Input.GetMouseButtonDown(0))
        {
            BoxSelection.Instance.mouseButtonWentDown();
        }

        if (Input.GetMouseButton(0))
        {
            BoxSelection.Instance.MouseButtonIsDown();
        }

        if (Input.GetMouseButtonUp(0))
        {
            BoxSelection.Instance.mouseButtonWentUp();
        }
    }

    private void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isShiftDown = true;
        }
        else (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isShiftDown = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            isControlDown = true;
        }
        else (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl))
        {
            isControlDown = false;
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

    protected virtual void OnSelectedControlGroupChanged()
    {
        SelectedControlGroupChanged?.Invoke(this, EventArgs.Empty);
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

        if (selectionBox.xMax > selectionBox.xMin && selectionBox.yMax > selectionBox.yMin) {
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

        SelectedControlGroup = -1;
        OnSelectedControlGroupChanged();
    }

    private void AddControlGroup(int keyNumber)
    {
        ControlGroups.AddGroup(keyNumber, _selectedUnits.Select(n => n.selectableObject).ToList());

        SelectedControlGroup = keyNumber;

        OnSelectedControlGroupChanged();
    }

    public void SelectControlGroup(int keyNumber)
    {
        DeselectAllUnits();

        if (ControlGroups.GetGroup(keyNumber) == null || ControlGroups.GetGroup(keyNumber).Count == 0)
        {
            SelectedControlGroup = -1;

            return;
        }

        foreach (var selectableObject in ControlGroups.GetGroup(keyNumber))
        {
            SelectUnit(selectableObject.unit, true);
        }

        OnSelectedControlGroupChanged();
    }
}
