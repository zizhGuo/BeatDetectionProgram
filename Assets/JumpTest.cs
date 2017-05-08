using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTest : MonoBehaviour
{
    public float jumpSpeed = 500f;
    bool grounded = true;
    float time = 0f;
    // Use this for initialization
    void Start()
    {

    }

    void Jump()
    {
        if (grounded == true)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed);
            grounded = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.deltaTime;
        //if (time = )
        //{
        //    Jump();
        //}
    }
    private void OnCollisionEnter(Collision collision)
    {
        grounded = true;

    }
}
