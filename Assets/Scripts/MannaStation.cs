using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MannaStation : MonoBehaviour
{
    [SerializeField] int chargesLeft;
    [SerializeField] Sprite emptySprite;
    [SerializeField] SpriteRenderer mannaStationSpriteRenderer;

    public bool StationTouched()
    {
        if (chargesLeft <= 0)
        {
            if (mannaStationSpriteRenderer.sprite != emptySprite)
            {
                mannaStationSpriteRenderer.sprite = emptySprite;
            }
            return false;
        }
        else
        {
            chargesLeft -= 1;
            if (chargesLeft <= 0)
            {
                mannaStationSpriteRenderer.sprite = emptySprite;
            }
            return true;
        }
    }
}
