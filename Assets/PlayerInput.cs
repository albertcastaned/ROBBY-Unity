using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{

    Player player;
    float shootTimer;
    bool canShoot;

    void Start()
    {   
        player = GetComponent<Player>();
        canShoot = true;
        shootTimer = 0.25f;
        
    }

    
    void Update()
    {
        if (GlobalVariables.pause == true)
            return;

        
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (directionalInput.x > 0)
            directionalInput.x = 1;
        if (directionalInput.x < 0)
            directionalInput.x = -1;
        player.SetDirectionalInput(directionalInput);

        if(!canShoot)
        {
            shootTimer -= Time.deltaTime;
            if(shootTimer < 0)
            {
                canShoot = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Jump"))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space) || Input.GetButtonUp("Jump"))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetButtonUp("Fire3"))
        {
            player.OnRoll();
        }
        
        if (Input.GetMouseButton(0) && canShoot)
        {
            player.ShootBullet();
            canShoot = false;
            shootTimer = 0.25f;
        }
        if ((Input.GetMouseButtonDown(1) || Input.GetButtonUp("Fire2")) && directionalInput.y >0 && player.velocity.y == 0)
        {
            player.Melee(1);
        }
        if ((Input.GetMouseButtonDown(1) || Input.GetButtonUp("Fire2")) && directionalInput.y < 0 )
        {
            player.Melee(2);
        }
        if ((Input.GetMouseButtonDown(1) || Input.GetButtonUp("Fire2")) && directionalInput.y > 0 && player.velocity.y != 0)
        {
            player.Melee(3);
        }
    }
}