using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collided with object");
        if (collision.gameObject.tag == "projectile")
        {
            Destroy(this.gameObject);
        }
    }
}
