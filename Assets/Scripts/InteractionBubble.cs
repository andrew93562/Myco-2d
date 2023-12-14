using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBubble : MonoBehaviour
{
    public GameEvent EnterInteraction;
    public GameEvent ExitInteraction;
    public GameEvent ChangeLevel;
    public GameEvent MannaRestored;
    public GameEvent UpdateSpawn;

    private void OnTriggerEnter2D(Collider2D other)
    {
        /*
        if (other.gameObject.CompareTag("interactableObject"))
        {
            EnterInteraction.Raise(this, other.gameObject.GetComponent<DialogueObject>());
        }
        */
        /*
        if (other.gameObject.CompareTag("LevelTrigger"))
        {
            ChangeLevel.Raise(this, other.gameObject.GetComponent<LevelTrigger>());
        }
        
        if (other.gameObject.CompareTag("mannaStation"))
        {
            bool mannaRestored = other.gameObject.GetComponent<MannaStation>().StationTouched();
            if (mannaRestored)
            {
                MannaRestored.Raise(this, null);
            }
        }
        */
        /*
        if (other.gameObject.CompareTag("Respawn"))
        {
            UpdateSpawn.Raise(this, other.gameObject.transform.position);
        }
        */
    }
    /*
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "interactableObject")
        {
            ExitInteraction.Raise(this, null);
        }
    }
    */
}
