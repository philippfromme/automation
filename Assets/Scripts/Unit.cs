using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent navMeshAgent;

    [SerializeField]
    private Animator animator;

    private bool isMoving = false;

    public SelectableObject selectableObject;

    private void Start()
    {
        GameManager gameManager = FindObjectOfType<GameManager>();

        if (gameManager != null)
        {
            gameManager.AddUnit(this);
        }

        selectableObject = GetComponent<SelectableObject>();
    }

    private void Update()
    {
        if (isMoving)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                animator.SetBool("isRunning", false);
                isMoving = false;
            }
        }
    }

    public void Select()
    {
        if (selectableObject != null)
        {
            selectableObject.ShowSelectionSprite();
        }
    }

    public void Deselect()
    {
        if (selectableObject != null)
        {
            selectableObject.HideSelectionSprite();
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.SetDestination(destination);

            animator.SetBool("isRunning", true);

            isMoving = true;
        }
    }
}
