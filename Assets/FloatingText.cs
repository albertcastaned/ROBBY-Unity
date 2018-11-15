using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour {
    public float DestroyTime = 1f;
    public Vector3 offset = new Vector3(0, 0.2f, 0);
	// Use this for initialization
	void Start () {
        Destroy(gameObject, DestroyTime);
        transform.localPosition += offset;
        transform.localPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0, 0);
	}
	

}
