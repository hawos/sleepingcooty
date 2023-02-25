using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCamera : MonoBehaviour
{
    [SerializeField]
    GameObject player;

    [SerializeField]
    float timeOffset;

    [SerializeField]
    Vector2 posOffset;

    [SerializeField]
    float leftLimit;

    [SerializeField]
    float rightLimit;
    
    [SerializeField]
    float topLimit;
    
    [SerializeField]
    float bottomLimit;

    private Vector3 velocity;

    private bool active = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init()
    {
        active = true;
        // set player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) {
            return;
        }
        
        //cameras current position
        Vector3 startPos = transform.position;

        //Players current position
        Vector3 endPos = player.transform.position;

        endPos.x += posOffset.x;
        endPos.y += posOffset.y;
        endPos.z -= 10;

        //lerp towards player
        //transform.position = Vector3.Lerp(startPos, endPos, timeOffset * Time.deltaTime);

        //SmoothDamp towards player
        transform.position = Vector3.SmoothDamp(startPos, endPos, ref velocity, timeOffset);


        transform.position = new Vector3
        (
            Mathf.Clamp(transform.position.x, leftLimit, rightLimit),
            Mathf.Clamp(transform.position.y, bottomLimit, topLimit),
            transform.position.z
        );
    }
}
