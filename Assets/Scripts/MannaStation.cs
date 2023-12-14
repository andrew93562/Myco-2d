using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannaStation : MonoBehaviour
{
    [SerializeField] public int chargesLeft;
    [SerializeField] Sprite emptySprite;
    [SerializeField] SpriteRenderer mannaStationSpriteRenderer;

    public void OnMannaRestored(Component sender, object data)
    {
        if (sender.GetComponent<MannaStation>() == this)
        {
            chargesLeft -= 1;
            //Debug.Log("manna station touched");
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
}
