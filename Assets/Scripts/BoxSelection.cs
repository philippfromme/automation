using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSelection : MonoBehaviour
{
    public static BoxSelection Instance { get; private set; }

    [SerializeField] private RectTransform selectionBoxTransform;
    [SerializeField] private Rect seletionBox;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private bool wasDragging = false;
    private bool mouseWentDown = false;

    public void Start()
    {
        if (Instance == null)
        {
            Instance = this;

            selectionBoxTransform.gameObject.SetActive(false);
        }
    }

    public void mouseButtonWentUp() {
        if (mouseWentDown && wasDragging) {
            mouseWentDown = false;
            wasDragging = false;

            startPosition = Vector2.zero;
            endPosition = Vector2.zero;

            DrawSelectionBox();

            GameManager gameManager = FindObjectOfType<GameManager>();
            
            if (gameManager != null) {
                gameManager.BoxSelect(seletionBox);
            }
        }
    }

    public void mouseButtonWentDown() {
        mouseWentDown = true;

        startPosition = Input.mousePosition;

        seletionBox = new Rect(startPosition.x, startPosition.y, 0, 0);

        selectionBoxTransform.gameObject.SetActive(true);

        DrawSelectionBox();
    }

    public void MouseButtonIsDown() {
        wasDragging = true;

        endPosition = Input.mousePosition;

        DrawSelectionBox();

        SetSelectionValues();
    }

    private void DrawSelectionBox() {
        Vector2 selectionBoxCenter = (startPosition + endPosition) / 2;

        selectionBoxTransform.position = selectionBoxCenter;

        Vector2 selectionBoxSize = new Vector2(Mathf.Abs(startPosition.x - endPosition.x), Mathf.Abs(startPosition.y - endPosition.y));

        selectionBoxTransform.sizeDelta = selectionBoxSize;
    }

    private void SetSelectionValues() {
        if (Input.mousePosition.x < startPosition.x) {
            seletionBox.xMin = Input.mousePosition.x;
            seletionBox.xMax = startPosition.x;
        } else {
            seletionBox.xMin = startPosition.x;
            seletionBox.xMax = Input.mousePosition.x;
        }

        if (Input.mousePosition.y < startPosition.y) {
            seletionBox.yMin = Input.mousePosition.y;
            seletionBox.yMax = startPosition.y;
        } else {
            seletionBox.yMin = startPosition.y;
            seletionBox.yMax = Input.mousePosition.y;
        }
    }
}
