using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : BaseAbilityController
{
    private GameObject player;
    private Animator animator;
    private SpriteRenderer playerSprite;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        animator = GetComponent<Animator>();
        animator.Play("Stick_Slash");
        playerSprite = player.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    new void Update()
    {
        float x = 1.5f;
        if(playerSprite.flipX) {
            spriteRenderer.flipX = false;
            x -= 3f;
        }
        else {
            spriteRenderer.flipX = true;
        }
        Vector2 target = new Vector2(player.transform.position.x + x, player.transform.position.y + 1.5f);
        transform.position = Vector2.MoveTowards(transform.position, target, 15f * Time.deltaTime);
    }

    public void AddCollider()
    {
        BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(1.41f, 1.79f);
        boxCollider.offset = new Vector2(-0.3f, 0.07f);
        boxCollider.isTrigger = true;
    }

    public void RemoveCollider()
    {
        BoxCollider2D boxCollider = gameObject.GetComponent<BoxCollider2D>();
        Destroy(boxCollider);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy")) {
            collider.GetComponent<EnemyHealth>().Damage(damage, true, player.transform.position);
        }
        else if(collider.CompareTag("Ludwig"))
        {
            collider.GetComponent<Ludwig>().Damage(damage);
        }
    }
}
