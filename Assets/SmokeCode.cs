using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeCode : MonoBehaviour {
    float speed;
    public Rigidbody2D rb;
    float tempX;
    // Use this for initialization
    void Start () {
        Vector3 v = Quaternion.AngleAxis(Random.Range(0.0f, 360f), Vector3.forward) * Vector3.up ;
        rb.velocity = v * 0.2f;
        tempX = 1 + Random.Range(0f, 1f);
        transform.localScale = new Vector3(tempX, tempX, 1);

    }
	
	// Update is called once per frame
	void Update () {
        if (transform.localScale.x < 0)
            Destroy(gameObject);
        tempX -= 0.003f;
        transform.localScale = new Vector3(tempX, tempX, 1);

    }

}
