using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour {
    public int health = 100;

    public float maxJumpHeight = 1;
    public float minJumpHeight = 0.2f;

    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = .07f;
    public float accelerationTimeGrounded = .04f;
    public float airFric = .07f;
    float moveSpeed = 1;
    float maxJumpVelocity;
    public GameObject deathEffect;
    float gravity;
    public Player player;
    Vector3 velocity;
    Controller2D controller;
    Vector2 directionalInput;
    int facing;
    bool damaged;
    float damagedTimer;
    SpriteRenderer tempSprite;
    void Start()
    {
        tempSprite = GetComponent<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        directionalInput.x = 0;
        directionalInput.y = 0;
        facing = -1;
        damaged = false;

    }
    public void DamageTimer()
    {
        damagedTimer -= Time.deltaTime;
        if (damagedTimer < 0)
        {
            damaged = false;
            tempSprite.color = Color.white;
        }
    }
    public void AI()
    {
        if ((Vector3.Distance(player.transform.position, transform.position) < 10f) && !damaged)
        {
            SetDirectionalInput(-1);

        }
        else
        {
            SetDirectionalInput(0);
        }
    }
    void Update()
    {
        if (GlobalVariables.pause == true)
            return;

        if (damaged)
            DamageTimer();

        // AI();
        CalculateVelocity();
        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }
    public void Jump()
    {
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }
    public void SetDirectionalInput(int input)
    {

        directionalInput.x = input;
        if (directionalInput.x == 1)
            facing = 1;

        else if ((directionalInput.x == -1))
            facing = -1;
    }
    public void TakeDamage(int damage, float vx, float vy)
    {

        if (!damaged)
        {
            health -= damage;
            damaged = true;
            tempSprite.color = Color.red;



            damagedTimer = 0.25f;
            velocity.x += vx + (Random.Range(0.2f, 0.5f) * Mathf.Sign(vx));
            velocity.y += vy;

            if (health <= 0)
                Die();
        }


    }

    float Approach(float argument0, float argument1, float argument2)
    {
        if (argument0 < argument1)
            return Mathf.Min(argument0 + argument2, argument1);
        else
            return Mathf.Max(argument0 - argument2, argument1);

    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        
        #region Normal
            if (controller.collisions.below)
            {

                velocity.x = Approach(velocity.x, targetVelocityX, accelerationTimeGrounded);

            }
            else
            {

                    velocity.x = velocity.x;
                

            }
        velocity.y += gravity * Time.deltaTime;
        #endregion
    }
    void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
