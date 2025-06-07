using System;
using UnityEngine;
using FishNet.Object;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DummyPlayerMovement : NetworkBehaviour
{
    public CharacterController characterController;
    public float movementSpeed = 5f;

    private Vector2 _currentMovementInput;
    

    public override void OnStartClient()
    {
        base.OnStartClient();

        GetComponent<PlayerInput>().enabled = IsOwner;
    }

    public void OnMove(InputValue value)
    {
        _currentMovementInput = value.Get<Vector2>();
    }

    private void Update()
    {
        UpdateMovement();
    }

    private void UpdateMovement_Old()
    {
        if (!IsOwner)
        {
            return;
        }

        Vector3 moveDirection = new Vector3(_currentMovementInput.x, 0f, _currentMovementInput.y);
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        transform.position += movementSpeed * Time.deltaTime * moveDirection;
    }

    private void UpdateMovement()
    {
        if (!IsOwner)
        {
            return;
        }
       
        Vector3 moveDirection = new Vector3(_currentMovementInput.x, 0f, _currentMovementInput.y);
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (characterController)
        {
            characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
        }
    }
}
