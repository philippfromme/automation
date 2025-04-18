using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxSelectionEnterEvent
{
    public SelectableObject Selectable { get; }
    public BoxSelectionEnterEvent(SelectableObject selectable) => Selectable = selectable;
}

public class BoxSelectionExitEvent
{
    public SelectableObject Selectable { get; }
    public BoxSelectionExitEvent(SelectableObject selectable) => Selectable = selectable;
}

public class BoxSelectionEvent
{
    public List<SelectableObject> SelectedObjects { get; }
    public BoxSelectionEvent(List<SelectableObject> selected) => SelectedObjects = selected;
}

public class BoxSelection : MonoBehaviour
{
    private RectTransform _selectionBoxTransform;

    private Vector2 _startPos;
    private Vector2 _currentPos;

    private bool _isSelecting;

    private readonly HashSet<SelectableObject> _selectedObjects = new();

    void OnEnable()
    {
        EventBus.Subscribe<MouseDownEvent>(OnMouseDownEvent);
        EventBus.Subscribe<MouseUpEvent>(OnMouseUpEvent);
        EventBus.Subscribe<MouseMoveEvent>(OnMouseMoveEvent);
    }

    void OnDisable()
    {
        EventBus.Unsubscribe<MouseDownEvent>(OnMouseDownEvent);
        EventBus.Unsubscribe<MouseUpEvent>(OnMouseUpEvent);
        EventBus.Unsubscribe<MouseMoveEvent>(OnMouseMoveEvent);
    }

    void OnMouseDownEvent(MouseDownEvent evt)
    {
        if (evt.Button != Mouse.MOUSE_BUTTON_LEFT) return;

        _isSelecting = true;

        _startPos = evt.Position;

        _currentPos = _startPos;
        
        UpdateSelectionBox();
        
        _selectionBoxTransform.gameObject.SetActive(true);
    }

    void OnMouseUpEvent(MouseUpEvent evt)
    {
        if (!_isSelecting || evt.Button != Mouse.MOUSE_BUTTON_LEFT) return;

        EventBus.Publish(new BoxSelectionEvent(new List<SelectableObject>(_selectedObjects)));

        _selectedObjects.Clear();

        _selectionBoxTransform.gameObject.SetActive(false);
        
        _isSelecting = false;
    }

    void OnMouseMoveEvent(MouseMoveEvent evt)
    {
        if (!_isSelecting) return;

        _currentPos = evt.Position;

        UpdateSelectionBox();

        UpdateSelection();
    }

    void UpdateSelectionBox()
    {
        _selectionBoxTransform.position = (_startPos + _currentPos) / 2;

        _selectionBoxTransform.sizeDelta = new Vector2(
            Mathf.Abs(_startPos.x - _currentPos.x),
            Mathf.Abs(_startPos.y - _currentPos.y)
        );
    }

    void UpdateSelection()
    {
        Vector3 worldPositionStart = GetWorldPosition(_startPos, Camera.main.nearClipPlane);
        Vector3 worldPositionEnd = GetWorldPosition(_currentPos, Camera.main.farClipPlane);

        Bounds selectionBounds = new Bounds();

        selectionBounds.SetMinMax(
            Vector3.Min(worldPositionStart, worldPositionEnd),
            Vector3.Max(worldPositionStart, worldPositionEnd)
        );

        Collider[] hits = Physics.OverlapBox(
            selectionBounds.center,
            selectionBounds.extents,
            Quaternion.identity
        );

        HashSet<SelectableObject> updatedSelectableObjects = new HashSet<SelectableObject>();

        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<SelectableObject>(out var selectableObject))
            {
                updatedSelectableObjects.Add(selectableObject);
            }
        }

        foreach (var selectableObject in _selectedObjects.Where(s => !updatedSelectableObjects.Contains(s)))
        {
            EventBus.Publish(new BoxSelectionExitEvent(selectableObject));
        }

        foreach (var selectableObject in updatedSelectableObjects.Where(s => !_selectedObjects.Contains(s)))
        {
            EventBus.Publish(new BoxSelectionEnterEvent(selectableObject));
        }

        _selectedObjects.Clear();

        _selectedObjects.UnionWith(updatedSelectableObjects);
    }

    Vector3 GetWorldPosition(Vector2 screenPosition, float zDepth = 0)
    {
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, zDepth)
        );

        return worldPosition;
    }
}
