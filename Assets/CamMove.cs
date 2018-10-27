using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMove : MonoBehaviour
{
    public Transform target;
    private Vector3 velocity = Vector3.zero;
    public float smoothSpeed = 0.125f;
    Vector3 offset;
    float shakeX = 0f;
    float shakeY = 0f;
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
            offset.z = -3f;
            offset.x = 0f;
            offset.y = 0f;
        }
        Vector3 random = new Vector3(shakeX, shakeY, 0);
        Vector3 desiredPosition = target.position + offset + random;
        Vector3 smoothedPosition = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothSpeed);
        transform.position = smoothedPosition;

        Vector3 rotation = target.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(rotation);


    }
}
