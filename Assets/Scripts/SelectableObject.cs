using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enums;

public class SelectableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverSprite;


    [SerializeField]
    private GameObject selectionSprite;

    [SerializeField]
    private SelectableType selectableType;

    [SerializeField]
    public Unit unit;

    private void Start()
    {
        if (selectableType == SelectableType.Unit)
        {
            unit = GetComponent<Unit>();
        }

        hoverSprite.SetActive(false);
        selectionSprite.SetActive(false);
    }

    private void OnMouseEnter()
    {
        hoverSprite.SetActive(true);
    }

    private void OnMouseExit()
    {
        hoverSprite.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (selectableType == SelectableType.Unit)
            {
                GameManager gameManager = FindObjectOfType<GameManager>();

                if (gameManager != null)
                {
                    gameManager.SelectUnit(unit, gameManager.isShiftDown);
                }
            }
        }
    }

    public void ShowSelectionSprite()
    {
        selectionSprite.SetActive(true);
    }

    public void HideSelectionSprite()
    {
        selectionSprite.SetActive(false);
    }
}   
