using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Pointer : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float pointerOffset = 2;
    [SerializeField] Sprite[] sprites;
    SpriteRenderer activeSpriteRenderer;
    // Update is called once per frame

    private void Start()
    {
        activeSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Vector2 rbPosition = new Vector2(player.transform.position.x, player.transform.position.y);
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos = new Vector2(mousePos3D.x, mousePos3D.y);
        Vector2 direction = -(mousePos - rbPosition).normalized;
        var angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        transform.localPosition = pointerOffset * direction;
        transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    public void OnProjectileCharging(Component sender, object data)
    {
        //Debug.Log("changing sprite");
        int projectileLevel = (int)data % sprites.Length;
        //Debug.Log(sprites[projectileLevel].name);
        activeSpriteRenderer.sprite = sprites[projectileLevel];
    }
}
