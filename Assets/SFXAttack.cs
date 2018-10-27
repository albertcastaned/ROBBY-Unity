using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXAttack : MonoBehaviour
{
    public Rigidbody2D rb;
    public SpriteRenderer spr;
    float tempX;
    float opacity;
    // Use this for initialization
    void Start()
    {
        opacity = 1f;
        Vector3 random = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.5f, 1f), 0);
        transform.localScale = random;
        tempX = 1;
        rb.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(0.1f, 1f), Random.Range(-2f, 2f));
        transform.Rotate(0f, 0f, Random.Range(-180f, 180f));



    }
    float Approach(float argument0, float argument1, float argument2)
    {
        if (argument0 < argument1)
            return Mathf.Min(argument0 + argument2, argument1);
        else
            return Mathf.Max(argument0 - argument2, argument1);

    }
    // Update is called once per frame
    void Update()
    {
        Vector3 update = new Vector3(Approach(rb.velocity.x, 0, 0.1f), Approach(rb.velocity.y, 0, 0.1f), 0);
        rb.velocity = update;
        if (transform.localScale.x < 0)
            Destroy(gameObject);
        tempX -= 0.025f;
        spr.color = new Color(1f, 1f, 1f, opacity);
        opacity -= 0.025f;
        transform.localScale = new Vector3(tempX, tempX, 1);

    }
}