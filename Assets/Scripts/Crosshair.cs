using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;
    SpriteRenderer activeSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        activeSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = -2;
        transform.position = mousePos;
        Cursor.visible = false;
    }

    public void OnProjectileCharging(Component sender, object data)
    {
        int projectileLevel = (int)data % sprites.Length;
        //Debug.Log(projectileLevel + " on crosshair");
        activeSpriteRenderer.sprite = sprites[projectileLevel];
    }
}