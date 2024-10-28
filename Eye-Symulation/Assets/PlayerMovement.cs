using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;

    PlayerInput playerInput;
    InputAction moveAction;
    Rigidbody rb;
    Transform orientation; 

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

        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z) * speed;

        // TODO: Poruszanie sie po wyboistym terenie
    }
}
