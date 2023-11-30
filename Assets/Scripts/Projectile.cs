using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float projectileSpeed = 5;
    Vector3 translateDirection;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 rbPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = (mousePos - rbPosition).normalized;
        translateDirection = new Vector3(direction.x, direction.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(translateDirection * projectileSpeed * Time.deltaTime);
    }
}
