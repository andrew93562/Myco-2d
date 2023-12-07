using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearingPlatform : MonoBehaviour
{
    [SerializeField] int ListensToSwitchID;
    public void OnActivate(Component sender, object data)
    {
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        int switchID = (int)data;
        if (ListensToSwitchID == switchID)
        {
            boxCollider.enabled = !boxCollider.enabled;
        }
    }
}
