using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{
    private bool active = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 movement;
    private float moveSpeed = 4f;
    private float lastDirectionX;
    private float lastDirectionY;

    private Vector2 fireDirection;

    private DiceController diceController;
    private AbilityController abilityController;

    private bool fire1 = false;
    private bool fire2 = false;

    private Transform playerTransform;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        if(rb) {
            rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
        }

        if(active) {
            abilityController.Fire(0, fireDirection);
            abilityController.Fire(1, fireDirection);
        }
    }

    public void Init()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        playerTransform = player.transform;

        rb = player.GetComponent<Rigidbody2D>();
        animator = player.GetComponent<Animator>();
        spriteRenderer = player.GetComponent<SpriteRenderer>();
        abilityController = player.GetComponent<AbilityController>();

        GameObject dice = GameObject.FindGameObjectWithTag("DiceContainer");
        diceController = dice.GetComponent<DiceController>();

        active = true;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if(active) {
            Vector2 inputVector = context.ReadValue<Vector2>();

            movement.x = inputVector.x;
            movement.y = inputVector.y;

            lastDirectionX = movement.x;
            lastDirectionY = movement.y;


            if(0 > lastDirectionX) {
                spriteRenderer.flipX = true;
            }
            else if(0 < lastDirectionX) {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if(!active) {
            return;
        }

        if(context.control.device is Mouse) {
            Vector2 direction = context.ReadValue<Vector2>();
            direction = Camera.main.ScreenToWorldPoint(direction);
            
            float x = (direction.x - playerTransform.position.x) * 100;
            float y = (direction.y - playerTransform.position.y) * 100;

            fireDirection = new Vector2(x, y);
        }
        else if(context.performed) {
            Vector2 direction = context.ReadValue<Vector2>();

            float x = (direction.x * 100 + playerTransform.position.x);
            float y = (direction.y * 100 + playerTransform.position.y);

            fireDirection = new Vector2(x, y);
        }

        // Debug.Log(fireDirection);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        // context.canceled
        if(active && context.performed) {
            fire1 = true;
        }
        else if(context.canceled) {
            fire1 = false;
        }
    }
    public void Fire2(InputAction.CallbackContext context)
    {
        // context.canceled
        if(active && context.performed) {
            fire2 = true;
        }
        else if(context.canceled) {
            fire2 = false;
        }
    }

    public void RollAll()
    {
        if(active) {
            diceController.RollAll();
        }
    }

    public void RollSelected()
    {
        // call Dice.RollSelected
    }

    public void Quit(InputAction.CallbackContext context)
    {
        if(context.performed) {
            MainManager.Instance.Quit();
        }
    }

    public void Reset()
    {
        active = false;
        rb = null;
        movement = new Vector3(0, 0, 0);
    }
}
