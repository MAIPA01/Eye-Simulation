using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float playerHeight;
    public float maxSlopeAngle;

    PlayerInput playerInput;
    InputAction moveAction;
    Rigidbody rb;
    Transform orientation;
    RaycastHit slopeHit;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        rb = GetComponent<Rigidbody>();
        orientation = GetComponent<Transform>();
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        Vector2 direction = moveAction.ReadValue<Vector2>();

        if (direction.sqrMagnitude != 0)
        {
            direction.Normalize();
        }

        Vector3 moveDirection = transform.forward * direction.y + transform.right * direction.x;

        if (OnSlope())
        {
            moveDirection = GetSlopeMoveDirection(moveDirection);
        }

        rb.velocity = moveDirection * speed;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }
}
