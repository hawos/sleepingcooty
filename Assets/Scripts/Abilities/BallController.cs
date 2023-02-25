using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : BaseAbilityController
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
        moveSpeed = 20f;
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy")) {
            collider.GetComponent<Enemy>().Slow(0.3f, 1f);
            collider.GetComponent<EnemyHealth>().Damage(damage, true, transform.position);
            Destroy(gameObject);
        }
        else if(collider.CompareTag("Ludwig"))
        {
            collider.GetComponent<Ludwig>().Damage(damage);
            Destroy(gameObject);
        }
    }
}
