using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PupsTrailController : BaseAbilityController
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    new void Update()
    {
        
    }

    protected void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Enemy")) {
            collider.GetComponent<Enemy>().Poison(damage, 5, 0.5f);
        }
        else if(collider.CompareTag("Ludwig"))
        {
            collider.GetComponent<Ludwig>().Poison(damage, 5, 0.5f);
        }
    }
}
