using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    [SerializeField] GameEvent SwitchActivated;
    [SerializeField] int SwitchID;

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collided with object");
        if (collision.gameObject.tag == "projectile")
        {
            SwitchActivated.Raise(this, SwitchID);
        }
    }
}
