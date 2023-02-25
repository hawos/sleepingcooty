using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurdController : BaseAbilityController
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy")) {
            if(animator) {
                animator.Play("Turd_Explosion");
            }
            collider.GetComponent<EnemyHealth>().Damage(damage, true, transform.position);
            Destroy(gameObject, 0.4f);
        }
        else if(collider.CompareTag("Ludwig"))
        {
            if(animator) {
                animator.Play("Turd_Explosion");
            }
            collider.GetComponent<Ludwig>().Damage(damage);
            Destroy(gameObject, 0.4f);
        }
    }
}
