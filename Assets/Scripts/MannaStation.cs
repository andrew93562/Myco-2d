using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannaStation : MonoBehaviour
{
    [SerializeField] public int chargesLeft;
    [SerializeField] Sprite emptySprite;
    [SerializeField] SpriteRenderer mannaStationSpriteRenderer;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        chargesLeft -= 1;
        if (chargesLeft <= 0)
        {
            if (mannaStationSpriteRenderer.sprite != emptySprite)
            {
                mannaStationSpriteRenderer.sprite = emptySprite;
            }
            //chargesLeft -= 1;
        }
        else
        {
            //chargesLeft -= 1;
            if (chargesLeft <= 0)
            {
                mannaStationSpriteRenderer.sprite = emptySprite;
            }
        }
    }
    /*
    public void OnMannaRestored(Component sender, object data)
    {
        //Debug.Log("manna station detects MannaRestored");
        Debug.Log(this + "is this");
        Debug.Log(sender.GetComponent<MannaStation>());
        if (sender.GetComponent<MannaStation>() == this)
        {
            chargesLeft -= 1;
            if (chargesLeft <= 0)
            {
                if (mannaStationSpriteRenderer.sprite != emptySprite)
                {
                    mannaStationSpriteRenderer.sprite = emptySprite;
                }
                //chargesLeft -= 1;
            }
            else
            {
                //chargesLeft -= 1;
                if (chargesLeft <= 0)
                {
                    mannaStationSpriteRenderer.sprite = emptySprite;
                }
            }
        }
        
    }
    */
}
