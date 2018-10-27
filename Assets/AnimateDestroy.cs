using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateDestroy : MonoBehaviour {
    public GameObject sfx;
	// Use this for initialization
	void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            Instantiate(sfx, transform.position, transform.rotation);
        }
        Destroy(gameObject, this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);

    }

    // Update is called once per frame
    void Update () {
		
	}
}
