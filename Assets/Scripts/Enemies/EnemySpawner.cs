using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    private int deaths = 0;
    private bool spawnBlue;
    private bool spawnRed;
    private Coroutine spawn;

    private void Awake()
    {
        if(null != Instance) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        spawn = StartCoroutine(SpawnEnemy(3f, "Schleimi", 100));
    }

    private IEnumerator SpawnEnemy(float interval, string enemy, int amount)
    {
        yield return new WaitForSeconds(interval);

        GameObject enemyPrefab = Resources.Load("Enemies/" + enemy) as GameObject;

        Vector3 spawnPosition = new Vector3(0,0,0);
        float positionY;

        /* Generate direction */
        Vector2 direction = new Vector2(Random.Range(0,4), Random.Range(0,3));

        if(0 == direction.y) {
            /* Start */
            positionY = Random.Range(10, 5);
        }
        else if(1 == direction.y) {
            /* Middle */
            positionY = Random.Range(5, -5);
        }
        else {
            /* End */
            positionY = Random.Range(-5, -14);
        }

        if(0 == direction.x) {
            /* Spawn on the left */
            spawnPosition.x = -18f;
            spawnPosition.y = positionY;
        }
        else if(1 == direction.x) {
            /* Spawn on the right */
            spawnPosition.x = 18f;
            spawnPosition.y = positionY;
        }
        else if(2 == direction.x) {
            /* Top */
            spawnPosition.x = positionY;
            spawnPosition.y = 18f;
        }
        else if(3 == direction.x) {
            /* Bottom */
            spawnPosition.x = positionY;
            spawnPosition.y = -18f;
        }

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;


        if(0 < (amount - 1)) {
            spawn = StartCoroutine(SpawnEnemy(interval, enemy, amount - 1));
        }
    }

    public void Death()
    {
        deaths++;

        if(deaths >= 50) {
            spawnBlue = true;
        }
        if(deaths >= 120) {
            spawnRed = true;
        }
    }

    public bool SpawnBlue()
    {
        return spawnBlue;
    }

    public bool SpawnRed()
    {
        return spawnRed;
    }

    public void Reset()
    {
        deaths = 0;
        spawnBlue = false;
        spawnRed = false;
        StopCoroutine(spawn);
    }
}
