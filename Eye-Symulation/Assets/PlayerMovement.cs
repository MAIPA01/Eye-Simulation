using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 1.0f;

    public Transform orientation;

    float horizontal;
    float vertical;

    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * vertical + orientation.right * horizontal;

        if (moveDirection.sqrMagnitude != 0)
        {
            moveDirection.Normalize();
        }

        rb.velocity = moveDirection * speed;
    }
}
