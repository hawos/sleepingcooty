using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogBoneController : BaseAbilityController
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector2(2f, 0);
        transform.SetParent(GameObject.FindGameObjectWithTag("PlayerRotate").transform, false);
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy")) {
            collider.GetComponent<EnemyHealth>().Damage(damage, true, transform.position);
        }
        else if(collider.CompareTag("Ludwig"))
        {
            collider.GetComponent<Ludwig>().Damage(damage);
        }
    }

}
