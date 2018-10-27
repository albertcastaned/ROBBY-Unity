using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour {

    public float speed = 300f;
    Vector3 mousePosition;
    Vector3 sp;
    Vector3 dir;
    public Rigidbody2D rb;
    public GameObject particles;
    public GameObject explo;
    Vector3 random;
    // Use this for initialization
    void Start () {
        sp = Camera.main.WorldToScreenPoint(transform.position);
        random.x = Random.Range(-20, 20);
        random.y = Random.Range(-20, 20);
        random.z = 0;
        dir = ((Input.mousePosition + random) - sp).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.Rotate(0f, 0f, angle);

    }

    void Update()
    {
        transform.position+= dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        Debug.Log(hitInfo.name);
        Enemy enemy = hitInfo.GetComponent<Enemy>();
        
        if (enemy!=null)
        {
            enemy.TakeDamage(20,1f * Mathf.Sign(dir.x),1f);
        }
        if (hitInfo.tag == "Solid" || hitInfo.tag == "Enemy")
        {
            for (int i = 0; i < 8; i++)
            {
                random = new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), transform.position.z);

                Instantiate(particles, transform.position + random, transform.rotation);
            }
            Instantiate(explo, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }

}
