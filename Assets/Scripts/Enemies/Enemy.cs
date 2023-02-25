using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject coots;
    private SpriteRenderer spriteRenderer;
    private float moveSpeed = 1f;
    private float _moveSpeed = 1f;
    private bool knockback = false;
    private Vector3 knockbackPosition;
    private float knockbackSpeed;
    private int knockbacks = 0;
    private bool active = false;

    // Start is called before the first frame update
    void Start()
    {
        coots = GameObject.FindGameObjectWithTag("Coots");
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(coots.transform.position.x < transform.position.x) {
            spriteRenderer.flipX = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(active) {
            if(knockback) {
                transform.position = Vector2.MoveTowards(transform.position, knockbackPosition, (knockbackSpeed * -1) * Time.deltaTime);
            }
            else {
                transform.position = Vector2.MoveTowards(transform.position, coots.transform.position, moveSpeed * Time.deltaTime);
            }
        }
        else {
            active = MainManager.Instance.HasGameStarted();
        }
    }

    public void Slow(float slow, float duration)
    {
        spriteRenderer.color = Color.cyan;
        
        moveSpeed -= slow;
        
        if(0 > moveSpeed) {
            slow = moveSpeed + slow;
            moveSpeed = 0f;
        }

        StartCoroutine(RemoveSlow(slow, duration));
    }

    public void Knockback(Vector3 knockbackPosition, float knockbackSpeed = 5f)
    {
        this.knockbackPosition = knockbackPosition;
        this.knockbackSpeed = knockbackSpeed;
        knockback = true;
        knockbacks++;
        StartCoroutine(StopKnockbacking(0.02f * knockbackSpeed));
    }

    private IEnumerator StopKnockbacking(float duration)
    {
        yield return new WaitForSeconds(duration);

        knockbacks--;

        if(0 == knockbacks) {
            knockback = false;
        }
    }

    private IEnumerator RemoveSlow(float slow, float duration)
    {
        yield return new WaitForSeconds(duration);

        moveSpeed += slow;

        if(moveSpeed == _moveSpeed) {
            spriteRenderer.color = Color.white;
        }
    }

    public void Poison(int damage, int ticks, float duration)
    {
        spriteRenderer.color = Color.green;
        StartCoroutine(ResetSpriteColor(duration));
        StartCoroutine(Ticks(damage, ticks, duration));
    }

    private IEnumerator Ticks(int damage, int ticks, float duration, float currentTick = 0)
    {
        yield return new WaitForSeconds(duration / ticks);
        currentTick++;
        gameObject.GetComponent<EnemyHealth>().Damage((int)(damage / ticks));

        if(ticks > currentTick) {
            StartCoroutine(Ticks(damage, ticks, duration, currentTick));
        }
    }

    private IEnumerator ResetSpriteColor(float duration)
    {
        yield return new WaitForSeconds(duration);

        spriteRenderer.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Coots")) {
            coots.GetComponent<CootsHealth>().Damage();
            Destroy(gameObject);
        }
    }
}
