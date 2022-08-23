using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public InputActionReference horizontalLook;
    public InputActionReference verticalLook;
    public InputActionReference movement;

    public float lookSpeed = 1f;
    public Transform cameraTransform;
    float pitch;
    float yaw;

    // WASD Movement
    Vector2 movementInput;
    Rigidbody body;
    public float speed;
    public float maxForce;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        horizontalLook.action.performed += HandleHorizontalLookChange;
        verticalLook.action.performed += HandleVerticalLookChange;

        body = GetComponent<Rigidbody>();
    }

    void HandleHorizontalLookChange(InputAction.CallbackContext obj)
    {
        yaw += obj.ReadValue<float>();
        transform.localRotation = Quaternion.AngleAxis(yaw * lookSpeed, Vector3.up);
    }

    void HandleVerticalLookChange(InputAction.CallbackContext obj)
    {
        pitch += obj.ReadValue<float>();
        cameraTransform.localRotation = Quaternion.AngleAxis(pitch * lookSpeed, Vector3.right);
    }

    void Update()
    {
        movementInput = movement.action.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 currentVelocity = body.velocity;
        Vector3 targetVelocity = new Vector3(movementInput.x, 0, movementInput.y);
        targetVelocity = targetVelocity * speed;

        Vector3 velocityChange = (targetVelocity - currentVelocity);
        Vector3.ClampMagnitude(velocityChange, maxForce);
        body.AddForce(velocityChange, ForceMode.VelocityChange);
    }



}
