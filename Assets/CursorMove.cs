using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorMove : MonoBehaviour {

    Vector3 screenPoint;
    float rotate;
    void Start()
    {
        screenPoint.z = 3f;
        rotate = 0;
        
    }
    // Update is called once per frame
    void Update()
    {
        screenPoint = Input.mousePosition;
        screenPoint.z = transform.position.z - Camera.main.transform.position.z;
        screenPoint.z = 3f;
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
        transform.Rotate(0f, 0f, 1f);

    }
}
