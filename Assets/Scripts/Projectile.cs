using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 5;
    [SerializeField] private float distanceToDespawn;
    private Vector3 translateDirection;
    private Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 rbPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = (mousePos - rbPosition).normalized;
        translateDirection = new Vector3(direction.x, direction.y, 0);
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(translateDirection * projectileSpeed * Time.deltaTime);
        Vector3 travelDistance = transform.position - startingPosition;
        transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        if (travelDistance.magnitude > distanceToDespawn)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("collision detected");
        if (collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
