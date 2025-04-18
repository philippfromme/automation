using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField]
    private GameObject hoverSprite;

    [SerializeField]
    private GameObject selectionSprite;

    [SerializeField]
    private SelectableType selectableType;

    private void Start()
    {
        hoverSprite.SetActive(false);
        selectionSprite.SetActive(false);

        EventBus.Subscribe<MouseEnterEvent>(OnMouseEnter);
        EventBus.Subscribe<MouseExitEvent>(OnMouseExit);
    }

    void OnDestroy()
    {
        EventBus.Unsubscribe<MouseEnterEvent>(OnMouseEnter);
        EventBus.Unsubscribe<MouseExitEvent>(OnMouseExit);
    }

    void OnMouseEnter(MouseEnterEvent evt)
    {
        hoverSprite.SetActive(true);
    }

    void OnMouseExit(MouseExitEvent evt)
    {
        hoverSprite.SetActive(false);
    }
}   
