using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{


    public float speed = 0.01f;
    private float xPos;


    void Start()
    {
        xPos = transform.position.x;
    }

    void Update()
    {
        transform.position = new Vector3(xPos + Mathf.Sin(Time.time * speed), transform.position.y, transform.position.z);
    }
}
