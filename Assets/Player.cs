

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {
    public float maxJumpHeight = 1;
    public float minJumpHeight = 0.2f;

    public float timeToJumpApex = 0.4f;
    public float accelerationTimeAirborne = .07f;
    public float accelerationTimeGrounded = .04f;
    public float airFric = .07f;
    float moveSpeed = 2.5f;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;


    public float wallSlideSpeedMax = 1;
    public float wallStickTime = .08f;
    float timeToUnstick;

    [HideInInspector]
    public float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;

    public Vector3 velocity;
    float velocityXSmoothing;
    float tempVelX;
    bool doubleJump;
    bool roll;
    bool rollback;

    public Transform firePoint;
    public GameObject bulletPrefab;
    
    [HideInInspector]
    public Controller2D controller;
    private Animator animator;
    private BoxCollider2D hitbox;
    private SpriteRenderer srender;
    bool wallSliding;
    int wallDirX;
    int facing;
    int originalFacing;
    bool auxFlip;
    public bool upFlip;
    Vector2 directionalInput;
    public bool rollAttack;
    bool onGroundPrev;

    public GameObject atk;
    public GameObject atk2;
    public GameObject atk3;
    public GameObject runSFX;
    void Start() {
        animator = GetComponentInChildren<Animator>();
        srender = GetComponentInChildren<SpriteRenderer>();
        controller = GetComponent<Controller2D>();
        hitbox = GetComponent<BoxCollider2D>();
        upFlip = false;
        rollAttack = false;
        auxFlip = false;

        doubleJump = false;
        roll = false;
        rollback = false;
        tempVelX = 0;
        facing = 1;
        onGroundPrev = true;
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        print("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
    }
    void Update() {
        
      
        srender.transform.eulerAngles = new Vector3(srender.transform.localRotation.x, srender.transform.localRotation.y,-1f* controller.collisions.slopeAngle);
        if(Mathf.Abs(velocity.x) > 10f)
        {
            rollAttack = true;
            srender.color = Color.red;
        }
        if(rollAttack && velocity.x == 0)
        {
            rollAttack = false;
            srender.color = Color.white;
        }

        if (GlobalVariables.pause == true)
        {
            animator.enabled = false;
            return;
        }
        else
        {
            animator.enabled = true;
        }

        if (roll || doubleJump)
        {
            Vector2 sizeH = new Vector2(0.170405f, 0.1993137f);
            Vector2 sizeO = new Vector2(0.003085591f, -0.1346828f);
            hitbox.size = sizeH;
            hitbox.offset = sizeO;
        }
        else
        {
            Vector2 sizeH = new Vector2(0.170405f, 0.3843396f);
            Vector2 sizeO = new Vector2(0.003085591f, -0.04216981f);
            hitbox.size = sizeH;
            hitbox.offset = sizeO;
        }


        CalculateVelocity();
        HandleWallSliding();
        UpdateAnimation();


        controller.Move(velocity * Time.deltaTime, directionalInput);


        if (controller.collisions.above || controller.collisions.below) {
            if (controller.collisions.slidingDownMaxSlope) {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            } else {
                velocity.y = 0;
                wallSlideSpeedMax = 1;
            }
        }
        if (controller.collisions.below)
        {
            doubleJump = false;

        }
        srender.transform.localScale = new Vector3(Approach(srender.transform.localScale.x, 1f, 0.02f), Approach(srender.transform.localScale.y, 1f, 0.02f), 1f);


    }
    public bool getRollAttack()
    {
        return rollAttack;
    }
    public int getFacing()
    {
        return facing;
    }
    public void Melee(int id)
    {
        Vector3 pos = transform.position;
        switch (id)
        {
            case 1:
                pos.x += 0.35f * facing;
                GameObject attack = Instantiate(atk, pos, transform.rotation) as GameObject;
                SpriteRenderer satk = attack.GetComponent<SpriteRenderer>();
                if (facing == -1)
                    satk.flipX = true;
                break;
                
            case 2:
                
                pos.x += 0.35f * facing;
                GameObject attack2 = Instantiate(atk2, pos, transform.rotation) as GameObject;
                SpriteRenderer satk2 = attack2.GetComponent<SpriteRenderer>();
                if (facing == -1)
                    satk2.flipX = true;
                break;
            case 3:

                pos.y += 0.2f;
                GameObject attack3 = Instantiate(atk3, pos, transform.rotation) as GameObject;
                SpriteRenderer satk3 = attack3.GetComponent<SpriteRenderer>();
                if (facing == -1)
                    satk3.flipX = true;
                break;
        }

    }

    
    public void SetDirectionalInput(Vector2 input) {

        directionalInput = input;
        if (!roll)
        {
            if (directionalInput.x == 1)
                facing = 1;

            else if ((directionalInput.x == -1))
                facing = -1;

            originalFacing = facing;

        }
    }
    public void ShootBullet()
    {
       // Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
    public void OnJumpInputDown()
    {
        srender.transform.localScale = new Vector3(1.33f, 0.86f, 1f);
        if (wallSliding)
        {
            wallSlideSpeedMax = 1;
            if (wallDirX == directionalInput.x)
            {
                velocity.y = wallJumpClimb.y;
                velocity.x = -wallDirX * wallJumpClimb.x;
            }
            else if (directionalInput.x == 0)
            {
                velocity.y = wallJumpOff.y;
                velocity.x += -wallDirX * wallJumpOff.x;
            }
            else
            {
                velocity.y = wallLeap.y;
                velocity.x = -wallDirX * wallLeap.x;

            }
            doubleJump = false;
        }

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
        else
        {
            if (!doubleJump && !wallSliding)
            {
                velocity.y = maxJumpVelocity;
                velocity.x = velocity.x;
                animator.SetBool("dJump", true);
                doubleJump = true;
            }
        }

    }

    public void OnJumpInputUp()
    {
        //srender.transform.localScale = new Vector3(1.33f,0.67f,1f);

        if (velocity.y > minJumpVelocity)
            velocity.y = minJumpVelocity;
    }
    public void OnRoll()
    {
        if ((!roll) && (!controller.collisions.left && !controller.collisions.right) && !upFlip)
        {
            roll = true;
            auxFlip = false;
            if (velocity.x == 0)
                tempVelX = facing * (velocity.x + 12);

            tempVelX = facing * (Mathf.Abs(velocity.x) + 4);
 
               

        }

	}


	void UpdateAnimation()
	{
        if (upFlip)
        {   if (!auxFlip)
                auxFlip = true;
            roll = false;
            animator.Play(Animator.StringToHash("PAirFlip"));
        }
        else if (roll && !auxFlip)
        {
                animator.Play(Animator.StringToHash("PRollJ"));
        }
        else
        {
            if (!wallSliding)
            {
                if (directionalInput.x > 0)
                    srender.flipX = false;
                else if (directionalInput.x < 0)
                    srender.flipX = true;
            }
            else
            {
                if (controller.collisions.left)
                {
                    srender.flipX = false;
                    facing = 1;
                }
                if (controller.collisions.right)
                {
                    srender.flipX = true;
                    facing = -1;
                }
            }
           

            if (controller.collisions.below)
            {
                auxFlip = false;
                if (directionalInput.x == 0)
                    animator.Play(Animator.StringToHash("PIdle"));
                else
                    animator.Play(Animator.StringToHash("PRun"));


            }
            else
            {
                if (wallSliding)
                {
                    animator.Play(Animator.StringToHash("PSlide"));
                }
                else if (doubleJump && !auxFlip)
                {

                    animator.Play(Animator.StringToHash("PRollJ"));
                }
                else
                {
                    if (velocity.y > 0)
                    {
                        animator.Play(Animator.StringToHash("PJumpU"));
                    }
                    else if (velocity.y > -0.5 && velocity.y < 0.5)
                    {
                        animator.Play(Animator.StringToHash("PJumpM"));
                    }
                    else if (velocity.y < 0)
                    {
                        animator.Play(Animator.StringToHash("PJumpD"));
                    }
                }
            }
        }



	}
	void HandleWallSliding()
	{
		wallSliding = false;
		wallDirX = (controller.collisions.left)? -1 : 1;
		if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
		{
			wallSliding = true;
			animator.SetBool("wallSlide",true);
			print("Wall sliding");
            for (int i = 0; i < 3; i++)
            {
                Vector3 aux = new Vector3(transform.position.x + -0.1f * facing, transform.position.y + 0.1f + Random.Range(0.01f, 0.03f), Random.Range(-0.5f, 0.5f));
                Instantiate(runSFX, aux, transform.rotation);
            }
            if (velocity.y < -wallSlideSpeedMax)
			{

                wallSlideSpeedMax += Time.deltaTime * 3;
				velocity.y = -wallSlideSpeedMax;
			}

			if(timeToUnstick > 0)
			{
				velocityXSmoothing = 0;
				velocity.x = 0;
				if(directionalInput.x != wallDirX && directionalInput.x !=0)
				{
				timeToUnstick -= Time.deltaTime;
				}else{
					timeToUnstick = wallStickTime;
				}
			}else{
				timeToUnstick = wallStickTime;
			}
		}else{
			animator.SetBool("wallSlide",false);
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
        if(controller.collisions.below && !onGroundPrev)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 aux = new Vector3(transform.position.x + Random.Range(-0.2f, 0.2f), transform.position.y - 0.2f + Random.Range(-0.01f, 0.01f), Random.Range(-0.5f,0.5f));
                Instantiate(runSFX, aux, transform.rotation);
            }
            srender.transform.localScale = new Vector3(1.23f, 0.83f, 1f);


            onGroundPrev = true;
        }
        if (!controller.collisions.below)
            onGroundPrev = false;
        #region Rolling
        if (controller.collisions.left || controller.collisions.right)
        {
            roll = false;
            tempVelX = 0;
            rollback = false;
            facing = originalFacing;
        }
        if (roll)
        {   
        
            float angularVel = (1 / Mathf.Cos(controller.collisions.slopeAngle * Mathf.Deg2Rad)) / 4;

            if (controller.collisions.slopeAngle == 0)
                rollback = false;
            if (controller.collisions.descendingSlope || rollback)
            {
                if (facing == 1)
                    tempVelX += angularVel;
                   
                else if (facing == -1)
                    tempVelX -= angularVel;
                    
            }
            else if (controller.collisions.climbingSlope)
            {
                tempVelX += angularVel * facing * -1.0f;


            }
            if(controller.collisions.slopeAngle == 0)
            tempVelX = Approach(tempVelX, 0, 6f * Time.deltaTime);
            velocity.x = tempVelX;
            
            if ((velocity.x <= 0 && facing == 1) || (velocity.x >= 0 && facing == -1))
            {
                
                if (controller.collisions.slopeAngle > 0)
                {
                    facing *= -1;
                    rollback = true;
                    animator.SetFloat("ReverseRoll", originalFacing * facing);

                }
                else
                {
                    roll = false;
                    tempVelX = 0;
                    rollback = false;
                    facing = originalFacing;
                }
            }

        }

        #endregion
        #region Normal
        else
        {
            
            if (controller.collisions.below)
            {

                velocity.x = Approach(velocity.x, targetVelocityX, accelerationTimeGrounded);
                if (Random.Range(0f, 100f) > 80f && Mathf.Abs(velocity.x) > 1f)
                {
                    Vector3 aux = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
                    Instantiate(runSFX, aux, transform.rotation);
                }
                
            }
            else
            {
                if (directionalInput.x < 0)
                {
                    if (velocity.x > 0)
                        velocity.x = Approach(velocity.x, 0, airFric);
                    velocity.x = Approach(velocity.x, -moveSpeed, accelerationTimeAirborne);
                }
                else if (directionalInput.x > 0)
                {
                    if (velocity.x < 0)
                        velocity.x = Approach(velocity.x, 0, airFric);
                    velocity.x = Approach(velocity.x, moveSpeed, accelerationTimeAirborne);

                }
                else
                {
                    velocity.x = velocity.x;
                }

            }
        }
		velocity.y += gravity * Time.deltaTime;
        #endregion
    }
    void OnGUI()
    {
        GUI.Label(new Rect(40, 50, 100, 20), velocity.x.ToString());
        GUI.Label(new Rect(40, 60, 100, 20), velocity.y.ToString());
        if(controller.collisions.climbingSlope)
        GUI.Label(new Rect(40, 70, 100, 20), "Climbing");
        if (controller.collisions.descendingSlope)
        GUI.Label(new Rect(40, 80, 100, 20), "Descending");
        GUI.Label(new Rect(40, 90, 100, 20), controller.collisions.slopeAngle.ToString());
        
        GUI.Label(new Rect(40, 110, 100, 20), rollback.ToString());
    }

}