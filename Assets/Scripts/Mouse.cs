using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseEventArgs : EventArgs
{
    public int mouseButton;
    public bool down;
    public bool up;
    public Vector2 downPosition;

    public MouseEventArgs(int mouseButton, bool down, bool up, Vector2 downPosition)
    {
        this.mouseButton = mouseButton;
        this.down = down;
        this.up = up;
        this.downPosition = downPosition;
    }
}

public class Mouse : MonoBehaviour
{
    public static Mouse Instance { get; private set; }

    public EventHandler MouseDown;
    public EventHandler MouseMove;
    public EventHandler MouseUp;

    public const int MouseButtonLeft = 0;
    public const int MouseButtonMiddle = 2;
    public const int MouseButtonRight = 1;

    private bool leftMouseButtonDown = false;
    private Vector2 leftMouseButtonDownPosition = Vector2.zero;

    private bool middleMouseButtonDown = false;
    private Vector2 middleMouseButtonDownPosition = Vector2.zero;

    private bool rightMouseButtonDown = false;
    private Vector2 rightMouseButtonDownPosition = Vector2.zero;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(MouseButtonLeft)) {
            leftMouseButtonDown = true;
            leftMouseButtonDownPosition = Input.mousePosition;
        }
    }

    protected virtual void OnMouseDown(KeyCode mouseButton)
    {
    }

    protected virtual void OnMouseUp(KeyCode mouseButton)
    {

    }

    protected virtual void OnMouseMove()
    {

    }
}
