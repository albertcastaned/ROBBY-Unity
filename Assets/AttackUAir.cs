﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackUAir : MonoBehaviour
{
    Animator anim;
    SpriteRenderer spr;
    float opacity;
    public GameObject particles;
    Vector3 random;
    Player pscript;
    int face;
    GameObject player;


    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");

        pscript = player.GetComponent<Player>();
        pscript.upFlip = true;
        face = pscript.getFacing();
        anim = GetComponent<Animator>();
        spr = GetComponent<SpriteRenderer>();
        opacity = 1f;
        for (int i = 0; i < 3; i++)
        {
            random = new Vector3(0.2f * face + Random.Range(-0.1f, 0.1f), 0.2f + Random.Range(-0.02f, -0.02f), transform.position.z);

            Instantiate(particles, transform.position + random, transform.rotation);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (pscript.controller.collisions.below)
        {
            pscript.upFlip = false;
            Destroy(gameObject);
        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.2f, player.transform.position.z);
        if (GlobalVariables.pause == true)
        {
            anim.enabled = false;
            return;
        }
        else
        {
            anim.enabled = true;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackUpAirDefaultA"))
        {
            opacity -= 0.03f;
            spr.color = new Color(1f, 1f, 1f, opacity);
        }
        if (opacity <= 0)
        {
            pscript.upFlip = false;
            Destroy(gameObject);
        }

    }

    IEnumerator Pause()
    {
        GlobalVariables.pause = true;

        yield return new WaitForSeconds(0.1f);

        GlobalVariables.pause = false;
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {

        Enemy enemy = hitInfo.GetComponent<Enemy>();

        if (enemy != null)
        {

            if (!spr.flipX)
                enemy.TakeDamage(20, 1.3f, 6f);
            else
                enemy.TakeDamage(20, -1.3f, 6f);

            StartCoroutine(Pause());
        }



    }
}
