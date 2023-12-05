using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionBubble : MonoBehaviour
{
    public GameEvent EnterInteraction;
    public GameEvent ExitInteraction;
    public GameEvent ChangeLevel;
    public GameEvent MannaRestored;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "interactableObject")
        {
            EnterInteraction.Raise(this, other.gameObject.GetComponent<InteractableObject>());
        }
        if (other.gameObject.tag == "LevelTrigger")
        {
            ChangeLevel.Raise(this, other.gameObject.GetComponent<LevelTrigger>());
        }
        if (other.gameObject.tag == "mannaStation")
        {
            bool mannaRestored = other.gameObject.GetComponent<MannaStation>().StationTouched();
            if (mannaRestored)
            {
                MannaRestored.Raise(this, null);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "interactableObject")
        {
            ExitInteraction.Raise(this, null);
        }
    }
}
