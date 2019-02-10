using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Transform target;
    public GameObject pobj;
    private Player player;
    private Vector3 velocity = Vector3.zero;
    public float smoothSpeed = 0.125f;
    Vector3 offset;
    float shakeX = 0f;
    float shakeY = 0f;
    int facing;
    float yPos;
    void Start()
    {
        yPos = 0f;
        player = pobj.GetComponent<Player>();
    }
    public void Shake()
    {
        shakeX = Random.Range(-1f, 1f);
        shakeY = Random.Range(-1f, 1f);
    }
    public void ResetShake()
    {

        shakeX = 0f;
        shakeY = 0f;
    }
    void LateUpdate()
    {

        facing = player.getFacing();

        if (player.velocity.y < -2f)
        {
            if (yPos > -4.6f)
                yPos += -2.5f * Time.deltaTime;
 

        }
        else
        {
            smoothSpeed = 0.125f;
            yPos = 0f;
        }
        if(GlobalVariables.pause == true)
        {
            Shake();
        }
        else
        {
            ResetShake();
        }

        if(target.rotation.y > 0)
        {
            offset.z = 0f;
            offset.x = -3f;
            offset.y = 0f;
        }else
        {
            offset.z = -5f;
            offset.x = 0f;
            offset.y = 0f;
        }
        Vector3 random = new Vector3(shakeX, shakeY, 0);
        Vector3 face = new Vector3(0.2f * facing, yPos, 0);

           
            Vector3 desiredPosition = target.position + offset + random + face;
            Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
                transform.position = smoothedPosition;
        
         

        Vector3 rotation = target.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);
            


    }
}
