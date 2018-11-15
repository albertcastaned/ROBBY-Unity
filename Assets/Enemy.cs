using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Enemy : MonoBehaviour {


    public float MaxHealth;
    public float CurrentHealth;

    public GameObject healthBarUI;
    public Slider slider;
    public Image sliderImg;

    public GameObject dmgText;
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

        CurrentHealth = MaxHealth;
        slider.value = CalculateHealth();
        tempSprite = GetComponentInChildren<SpriteRenderer>();
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
    float CalculateHealth()
    {
        return CurrentHealth / MaxHealth;
    }
    void Update()
    {
        slider.value = CalculateHealth();
        sliderImg.color = Color.Lerp(Color.red, Color.green, slider.value);
        if(CurrentHealth < MaxHealth)
        {
            healthBarUI.SetActive(true);
        }
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
        tempSprite.transform.localScale = new Vector3(Approach(tempSprite.transform.localScale.x, 1f, 0.04f), Approach(tempSprite.transform.localScale.y, 1f, 0.04f), 1f);

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
    public void TakeDamage(float damage, float vx, float vy)
    {
        tempSprite.transform.localScale = new Vector3(1.33f, 0.77f, 1f);
        if (!damaged)
        {
            CurrentHealth -= damage;
            damaged = true;
            tempSprite.color = Color.red;


            if(dmgText)
            {
                var txt = Instantiate(dmgText, transform.position, Quaternion.identity, transform);
                txt.GetComponent<TextMesh>().text = damage.ToString();
            }


            damagedTimer = 0.25f;
            velocity.x += vx + (Random.Range(0.2f, 0.5f) * Mathf.Sign(vx));
            velocity.y += vy;

            if (CurrentHealth <= 0)
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
            if (controller.collisions.right || controller.collisions.left)
                velocity.x = 0;
                

            }
        velocity.y += gravity * Time.deltaTime;
        #endregion
    }
    void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {

        Player player = hitInfo.GetComponent<Player>();

        if (player != null)
        {

            if(player.getRollAttack())
            {
                TakeDamage(20f, 3f * player.getFacing(), 1f);
            }
        }



    }
}
