using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    private List<Unit> _units;

    private List<Unit> _selectedUnits;

    public GameObject DestinationSprite;

    private void Start()
    {
        _units = new List<Unit>();
        _selectedUnits = new List<Unit>();
    }

    public void Update()
    {
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
}
