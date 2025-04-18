using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MouseEvent
{
    public Vector2 Position { get; protected set; }
}

public class MouseDownEvent : MouseEvent
{
    public int Button { get; }
    public MouseDownEvent(int button, Vector2 position)
    {
        Button = button;
        Position = position;
    }
}

public class MouseUpEvent : MouseEvent
{
    public int Button { get; }
    public Vector2 MouseDownPosition { get; }
    public MouseUpEvent(int button, Vector2 position, Vector2 mouseDownPosition)
    {
        Button = button;
        Position = position;
        MouseDownPosition = mouseDownPosition;
    }
}

public class MouseClickEvent : MouseEvent
{
    public int Button { get; }
    public MouseClickEvent(int button, Vector2 position)
    {
        Button = button;
        Position = position;
    }
}

public class MouseMoveEvent : MouseEvent
{
    public Vector2 PreviousPosition { get; }
    public Vector2 Delta { get; }
    public MouseMoveEvent(Vector2 position, Vector2 previousPosition)
    {
        Position = position;
        PreviousPosition = previousPosition;
        Delta = position - previousPosition;
    }
}

public class MouseEnterEvent : MouseEvent
{
    public GameObject EnteredGameObject { get; }
    public MouseEnterEvent(GameObject enteredGameObject, Vector2 position)
    {
        EnteredGameObject = enteredGameObject;
        Position = position;
    }
}

public class MouseExitEvent : MouseEvent
{
    public GameObject ExitedGameObject { get; }
    public MouseExitEvent(GameObject exitedGameObject, Vector2 position)
    {
        ExitedGameObject = exitedGameObject;
        Position = position;
    }
}

public class Mouse : MonoBehaviour
{
    public static Mouse Instance { get; private set; }

    public const int MOUSE_BUTTON_LEFT = 0;
    public const int MOUSE_BUTTON_MIDDLE = 2;
    public const int MOUSE_BUTTON_RIGHT = 1;

    private Vector2 _previousMousePosition;
    private Vector2 _mouseDownPosition;

    private GameObject _gameObject;

    private const float MOUSE_CLICK_THRESHOLD = 5f; // Pixels

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _previousMousePosition = Input.mousePosition;
    }

    public void Update()
    {
        CheckMouseButton(MOUSE_BUTTON_LEFT);
        CheckMouseButton(MOUSE_BUTTON_RIGHT);
        CheckMouseButton(MOUSE_BUTTON_MIDDLE);

        CheckMouseEnterOut();

        CheckMouseMove();
    }

    void CheckMouseButton(int button)
    {
        if (Input.GetMouseButtonDown(button))
        {
            _mouseDownPosition = Input.mousePosition;

            EventBus.Publish(new MouseDownEvent(button, _mouseDownPosition));
        }

        if (Input.GetMouseButtonUp(button))
        {
            Vector2 mouseUpPosition = Input.mousePosition;

            float distance = Vector2.Distance(_mouseDownPosition, mouseUpPosition);

            if (distance <= MOUSE_CLICK_THRESHOLD)
            {
                EventBus.Publish(new MouseClickEvent(button, mouseUpPosition));
            }

            EventBus.Publish(new MouseUpEvent(button, mouseUpPosition, _mouseDownPosition));
        }
    }

    void CheckMouseEnterOut()
    {
        if (IsOverUI()) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject gameObject = hit.collider.gameObject;

            if (gameObject != _gameObject)
            {
                if (_gameObject != null)
                {
                    EventBus.Publish(new MouseExitEvent(
                        _gameObject,
                        Input.mousePosition
                    ));
                }

                _gameObject = gameObject;

                EventBus.Publish(new MouseEnterEvent(
                    _gameObject,
                    Input.mousePosition
                ));
            }
        }
        else if (_gameObject != null)
        {
            EventBus.Publish(new MouseExitEvent(
                _gameObject,
                Input.mousePosition
            ));

            _gameObject = null;
        }
    }

    private void CheckMouseMove()
    {
        Vector2 mousePosition = Input.mousePosition;

        if (mousePosition != _previousMousePosition)
        {
            EventBus.Publish(new MouseMoveEvent(mousePosition, _previousMousePosition));

            _previousMousePosition = mousePosition;
        }
    }

    bool IsOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
