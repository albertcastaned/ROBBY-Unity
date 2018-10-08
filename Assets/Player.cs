

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {
	public float maxJumpHeight = 1;
	public float minJumpHeight = 0.2f;

	public float timeToJumpApex = 0.4f;
	public float accelerationTimeAirborne = .07f;
	public float accelerationTimeGrounded = .04f;
    public float airFric = .07f;
    float moveSpeed = 2;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;
   

	public float wallSlideSpeedMax = 1;
	public float wallStickTime = .08f;
	float timeToUnstick;

	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;

	Vector3 velocity;
	float velocityXSmoothing;
    float velocityYSmoothing;
	bool doubleJump;
	bool roll;

	Controller2D controller;
	public Animator animator;
	private SpriteRenderer srender;
	bool wallSliding;
	int wallDirX;

	Vector2 directionalInput;
	void Start() {
		srender = GetComponent<SpriteRenderer> ();
		controller = GetComponent<Controller2D> ();
		doubleJump = false;
		roll = false;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt(2*Mathf.Abs(gravity) * minJumpHeight);
		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);
	}
		void Update() {
		

		CalculateVelocity ();
		HandleWallSliding ();
		UpdateAnimation ();
	

		controller.Move(velocity * Time.deltaTime, directionalInput);


		if (controller.collisions.above || controller.collisions.below) {
			if (controller.collisions.slidingDownMaxSlope) {
				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
			} else {
				velocity.y = 0;
                wallSlideSpeedMax = 1;
            }
		}
		if(controller.collisions.below)
		{
			doubleJump = false;
			
		}


	}
	public void SetDirectionalInput(Vector2 input){
		directionalInput = input;
	}

	public void OnJumpInputDown()
	{
			
			if(wallSliding)
			{	wallSlideSpeedMax = 1;
				if(wallDirX == directionalInput.x)
				{
					velocity.y = wallJumpClimb.y;
					velocity.x = -wallDirX * wallJumpClimb.x;
				}
				else if(directionalInput.x==0)
				{
					velocity.y = wallJumpOff.y;
                    velocity.x += -wallDirX * wallJumpOff.x;
				}else{
					velocity.y = wallLeap.y;
					velocity.x = -wallDirX * wallLeap.x;
					
				}
			}

			if (controller.collisions.below) 
			{
				if (controller.collisions.slidingDownMaxSlope) {
					if (directionalInput.x != -Mathf.Sign (controller.collisions.slopeNormal.x)) { // not jumping against max slope
						velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
						velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
					}
				} else {
					velocity.y = maxJumpVelocity;
				}
			}else{
				if(!doubleJump && !wallSliding)
				{
					velocity.y = maxJumpVelocity;
					animator.SetBool("dJump",true);
					doubleJump = true;
				}
			}
			//doubleJump = true;
	}

	public void OnJumpInputUp()
	{
		if(velocity.y > minJumpVelocity)
			velocity.y = minJumpVelocity;
	}
	public void OnRoll()
	{
		roll = !roll;
	}


	void UpdateAnimation()
	{
		if(!wallSliding)
		{
		if(directionalInput.x==1)
			srender.flipX = false;
		else if(directionalInput.x==-1)
			srender.flipX = true;
		}else{
			if(controller.collisions.left)
			srender.flipX = false;
			if(controller.collisions.right)
			srender.flipX = true;
		}

		if(controller.collisions.below)
		{
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
            else if (doubleJump == true)
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
	void HandleWallSliding()
	{
		wallSliding = false;
		wallDirX = (controller.collisions.left)? -1 : 1;
		if((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0 && !doubleJump)
		{
			wallSliding = true;
			animator.SetBool("wallSlide",true);
			print("Wall sliding");
			if(velocity.y < -wallSlideSpeedMax)
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
        if (controller.collisions.below)
        {
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (velocity.x > 0)
                    velocity.x = Approach(velocity.x, 0, airFric);
                velocity.x = Approach(velocity.x, -moveSpeed, accelerationTimeAirborne);
            }
            else if (Input.GetKey(KeyCode.D))
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
		velocity.y += gravity * Time.deltaTime;
	}
}