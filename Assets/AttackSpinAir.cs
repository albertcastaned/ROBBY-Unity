using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpinAir : MonoBehaviour
{
    Animator anim;
    SpriteRenderer spr;
    float opacity;
    public GameObject particles;
    Vector3 random;
    Player pscript;
    int face;
    float tempvelx;
    GameObject player;


    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player");

        pscript = player.GetComponent<Player>();
        face = pscript.getFacing();
        tempvelx = pscript.velocity.x + (1*face);
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


        if (20 >= Random.Range(0, 100))
        {
            random = new Vector3(-0.2f * face + Random.Range(-0.1f, 0.1f), -0.1f + Random.Range(-0.02f, -0.02f), transform.position.z);
            Instantiate(particles, transform.position + random, transform.rotation);
        }

            

        

    pscript.velocity.x = tempvelx;
        if (pscript.controller.collisions.below)
        {
            pscript.upFlip = false;
            Destroy(gameObject);
        }
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z);
        if (GlobalVariables.pause == true)
        {
            anim.enabled = false;
            return;
        }
        else
        {
            anim.enabled = true;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("AttackAirSpin"))
        {
            opacity -= 0.015f;
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

        yield return new WaitForSeconds(0.04f);

        GlobalVariables.pause = false;
    }
    void OnTriggerEnter2D(Collider2D hitInfo)
    {

        Enemy enemy = hitInfo.GetComponent<Enemy>();

        if (enemy != null)
        {

            pscript.velocity.y = 3f;
            //if (!spr.flipX)
                enemy.TakeDamage(20, tempvelx, 3f,false);
           // else
              //  enemy.TakeDamage(20, -tempvelx, 3.2f, false);

            StartCoroutine(Pause());
        }



    }
}
