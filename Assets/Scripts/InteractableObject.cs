using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    private bool isInteracting;
    public bool needsInput;
    [SerializeField] GameEvent TriggerInteraction;
    [SerializeField] GameEvent EnterInteraction;
    [SerializeField] GameEvent ExitInteraction;


    void Update()
    {
        if (needsInput)
        {
            if (isInteracting && Input.GetKeyDown(KeyCode.W))
            {
                TriggerInteraction.Raise(this, null);
            }
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (needsInput)
            {
                isInteracting = true;
            }
            if (EnterInteraction != null)
            {
                EnterInteraction.Raise(this, null);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (needsInput)
            {
                isInteracting = false;
            }
            if (ExitInteraction != null)
            {
                ExitInteraction.Raise(this, null);
            }
        }
    }

}
