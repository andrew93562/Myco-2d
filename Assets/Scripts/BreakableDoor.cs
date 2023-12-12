using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableDoor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D otherRB = collision.GetComponent<Rigidbody2D>();
        Debug.Log(otherRB.velocity.magnitude);
        Collider2D[] _myColliders = GetComponents<Collider2D>();

        if (collision.gameObject.CompareTag("Player") && otherRB.velocity.magnitude > 50)
        {
            foreach (Collider2D collider in _myColliders)
            {
                collider.enabled = false;
            }
        }
    }
}
