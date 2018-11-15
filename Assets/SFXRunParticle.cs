using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXRunParticle : MonoBehaviour {
    SpriteRenderer spr;
    float opacity;
    float speed;
	// Use this for initialization
	void Start () {
        spr = GetComponent<SpriteRenderer>();
        float aux = Random.Range(0.02f, 1f);
        Vector3 random = new Vector3(aux, aux, aux);
        transform.localScale = random;
        transform.Rotate(Vector3.forward,Random.Range(0,360));
        opacity = 1f;
        speed = Random.Range(0.1f, 0.3f);
		
	}
	
	// Update is called once per frame
	void Update () {
        if (opacity < 0f)
            Destroy(gameObject);
        transform.position += transform.up * speed * Time.deltaTime;
        Vector3 update = new Vector3(transform.localScale.x + 0.033f, transform.localScale.y + 0.033f,  1f);
        transform.localScale = update;
        opacity -= 0.05f;
        spr.color = new Color(1f, 1f, 1f, opacity);
	}
}
