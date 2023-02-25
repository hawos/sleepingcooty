using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseAbilityController : MonoBehaviour
{
    public int damage = 30;
    protected Vector2 direction;
    protected Vector2 target;
    protected float moveSpeed = 3f;
    protected SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if(40f <= transform.position.x || -40f >= transform.position.x || 40f <= transform.position.y || -40f >= transform.position.y) {
            Destroy(gameObject);
        }
    }

    public void SetDirection(Vector2 direction)
    {
        target = direction;

        if(direction.x < 100 || direction.y < 100) {
            target = new Vector2(direction.x * 100, direction.y * 100);
        }

        moveSpeed = 10f;
    }
}
