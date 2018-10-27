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
        player.SetDirectionalInput(directionalInput);

        if(!canShoot)
        {
            shootTimer -= Time.deltaTime;
            if(shootTimer < 0)
            {
                canShoot = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.OnJumpInputUp();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            player.OnRoll();
        }
        if (Input.GetMouseButton(0) && canShoot)
        {
            player.ShootBullet();
            canShoot = false;
            shootTimer = 0.25f;
        }
        if (Input.GetMouseButtonDown(1) && directionalInput.y == 1)
        {
            player.Melee();
        }
        if (Input.GetMouseButtonDown(1) && directionalInput.y == -1)
        {
            player.Melee2();
        }
    }
}