using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ludwig : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool clockwise = true;
    private bool locked = false;
    public float angle = 4.078324f;
    private int radius = 7;
    private bool spawn = false;
    private bool active = false;
    private string currentAnimation;
    private string idleAnimation = "Ludwig_Idle";
    private string walkAnimation = "Ludwig_Walk";
    private string summonAnimation = "Ludwig_Summon";
    private float summonClipLength;

    private int health;
    public int maxHealth;

    private LudwigHealthbar ludwigHealthbar;

    // Sleeping Cooty
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ludwigHealthbar = GameObject.FindGameObjectWithTag("LudwigHealthbar").GetComponent<LudwigHealthbar>();
        ludwigHealthbar = GameObject.FindObjectsOfType<LudwigHealthbar>(true)[0];
        ludwigHealthbar.SetHealth(maxHealth, maxHealth);
        ludwigHealthbar.Hide();

        animator = gameObject.GetComponent<Animator>();
        SetAnimation(idleAnimation);

        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        foreach(AnimationClip clip in clips)
        {
            switch(clip.name)
            {
                case "Ludwig_Summon":
                    summonClipLength = clip.length;
                    break;
            }
        }
    }

    public void Init()
    {
        active = true;
        SetAnimation(walkAnimation);
        ludwigHealthbar.ShowHealthbar(maxHealth);

        int duration = Random.Range(6, 12);

        StartCoroutine(StartSpawning(duration));
    }

    // Update is called once per frame
    void Update()
    {
        if(!active) {
            return;
        }
        if(spawn) {
            return;
        }

        if(!locked) {
            locked = true;
            StartCoroutine(UnlockDirection());

            /* Change direction */
            if(0 == Random.Range(0,5)) {
                clockwise = !clockwise;
            }
        }

        float x = radius * Mathf.Cos(angle);
    
        /* Flip sprite */
        if(x < transform.position.x) {
            spriteRenderer.flipX = true;
        }
        else if(x > transform.position.x) {
            spriteRenderer.flipX = false;
        }

        transform.position = new Vector2(x, radius * Mathf.Sin(angle) - 2f);
    
        if(clockwise) {
            angle -= 15 * Mathf.Deg2Rad * Time.deltaTime;
        }
        else {
            angle += 15 * Mathf.Deg2Rad * Time.deltaTime;
        }
    }

    public void Damage(int amount)
    {
         if(0f >= amount) {
            return;
        }

        AudioController.Instance.LudwigHit();

        health -= amount;

        ludwigHealthbar.SetHealth((float)health, (float)maxHealth);

        if(0 >= health) {
            MainManager.Instance.Win();
        }
    }

    private IEnumerator SummonEnemies()
    {
        yield return new WaitForSeconds(0.85f);

        float interval = 0.15f;
        int amount = Random.Range(2, 5);

        if(EnemySpawner.Instance.SpawnBlue()) {
            if(EnemySpawner.Instance.SpawnRed()) {
                StartCoroutine(SpawnEnemy("Schleimirot", interval, 1, 8));
                StartCoroutine(SpawnEnemy("Schleimiblau", interval, 2, 6));
                amount -=2;

                if(amount <= 0) {
                    amount = 1;
                }
            }
            else {
                StartCoroutine(SpawnEnemy("Schleimiblau", interval, 1, 6));
            }
        }
        
        StartCoroutine(SpawnEnemy("Schleimi", interval, amount));
    }

    private IEnumerator StartSpawning(int duration)
    {
        yield return new WaitForSeconds(duration);

        spawn = true;
        SetAnimation(summonAnimation);
        StartCoroutine(StopSpawning(summonClipLength));
        StartCoroutine(SummonEnemies());

        duration = Random.Range(6, 12);

        StartCoroutine(StartSpawning(duration));
    }

    private IEnumerator SpawnEnemy(string enemy, float interval, int amount, int iteration = 0)
    {
        yield return new WaitForSeconds(interval);

        GameObject enemyPrefab = Resources.Load("Enemies/" + enemy) as GameObject;

        List<Vector2> spawnPositions = new List<Vector2>() {
            new Vector2(-1f, -1.5f),
            new Vector2(-2f, 3f),
            new Vector2(2.4f, 1.45f),
            new Vector2(0.5f, -1.15f),
            new Vector2(-3.6f, 1.5f),
            new Vector2(2f, -0.25f),
            new Vector2(-2.8f, -1f),
            new Vector2(0f, 4f),
            new Vector2(2f, 3.15f),
            new Vector2(-3f, 0.2f),
        };

        Vector3 spawnPosition = new Vector3(gameObject.transform.position.x + spawnPositions[iteration].x, gameObject.transform.position.y + spawnPositions[iteration].y, 0);

        GameObject newEnemy;
        newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity) as GameObject;
        
        if(0 < (amount - 1)) {
            StartCoroutine(SpawnEnemy(enemy, interval, amount - 1, ++iteration));
        }
    }

    private IEnumerator StopSpawning(float duration)
    {
        yield return new WaitForSeconds(duration);

        spawn = false;
        SetAnimation(walkAnimation);
    }

    private IEnumerator UnlockDirection()
    {
        yield return new WaitForSeconds(2f);

        locked = false;
    }

    private void SetAnimation(string animation)
    {
        animator.Play(animation);
        currentAnimation = animation;
    }

    public void Poison(int damage, int ticks, float duration)
    {
        StartCoroutine(Ticks(damage, ticks, duration));
    }

    private IEnumerator Ticks(int damage, int ticks, float duration, float currentTick = 0)
    {
        yield return new WaitForSeconds(duration / ticks);
        currentTick++;
        Damage((int)(damage / ticks));

        if(ticks > currentTick) {
            StartCoroutine(Ticks(damage, ticks, duration, currentTick));
        }
    }
}
