using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ControlGroups
{
    private const int _maxGroups = 10;

    private Dictionary<int, List<SelectableObject>> _controlGroups = new Dictionary<int, List<SelectableObject>>();

    private ILogger _logger;

    public ControlGroups(ILogger logger)
    {
        _logger = logger;
    }

    public void AddGroup(int groupNumber, List<SelectableObject> selectableObjects)
    {
        if (groupNumber < 0 || groupNumber >= _maxGroups)
        {
            throw new System.ArgumentOutOfRangeException("Group number must be between 0 and " + (_maxGroups - 1));
        }

        _controlGroups[groupNumber] = selectableObjects;

        var groupsToRemove = new List<int>();

        foreach (var group in _controlGroups)
        {
            foreach (var selectableObject in selectableObjects)
            {
                if (group.Key != groupNumber && group.Value.Contains(selectableObject))
                {
                    _logger.Log("Removing selectable object from group: " + group.Key);

                    group.Value.Remove(selectableObject);
                }
            }

            if (group.Value.Count == 0)
            {
                groupsToRemove.Add(group.Key);
            }
        }

        foreach (var groupNumberToRemove in groupsToRemove)
        {
            _logger.Log("Removing empty group: " + groupNumberToRemove);

            _controlGroups.Remove(groupNumberToRemove);
        }

        _logger.Log("Groups after adding group " + groupNumber + ":");
        foreach (var group in _controlGroups)
        {
            _logger.Log("Group " + group.Key + ": " + string.Join(", ", group.Value.Count));
        }
    }

    public void RemoveGroup(int groupNumber)
    {
        if (groupNumber < 0 || groupNumber >= _maxGroups)
        {
            throw new System.ArgumentOutOfRangeException("Group number must be between 0 and " + (_maxGroups - 1));
        }

        if (_controlGroups.ContainsKey(groupNumber))
        {
            _controlGroups.Remove(groupNumber);
        }
    }

    public List<SelectableObject> GetGroup(int groupNumber)
    {
        if (groupNumber < 0 || groupNumber >= _maxGroups)
        {
            throw new System.ArgumentOutOfRangeException("Group number must be between 0 and " + (_maxGroups - 1));
        }

        if (_controlGroups.ContainsKey(groupNumber))
        {
            return _controlGroups[groupNumber];
        }

        return null;
    }
}
