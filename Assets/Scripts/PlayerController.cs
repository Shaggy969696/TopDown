using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Controls player movement and rotation for a 3D top-down shooter prototype.
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Movement speed in units per second.")]
    private float moveSpeed = 5f;

    [SerializeField]
    [Tooltip("Initial upward velocity applied when jumping.")]
    private float jumpForce = 5f;

    [SerializeField]
    [Tooltip("Gravity applied manually because CharacterController does not apply physics gravity by itself.")]
    private float gravity = -9.81f;

    [SerializeField]
    [Tooltip("Rotation interpolation speed.")]
    private float rotationSpeed = 20f;

    private Camera mainCamera;
    private CharacterController characterController;
    private Vector2 moveInput;
    private Vector3 lookTarget;
    private float verticalVelocity;
    private bool isJumping;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded)
        {
            verticalVelocity = jumpForce;
            isJumping = true;
        }
    }

    public void MouseLook(InputAction.CallbackContext context)
    {
        Vector2 mouseScreenPosition = context.ReadValue<Vector2>();

        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (groundPlane.Raycast(ray, out float enter))
        {
            lookTarget = ray.GetPoint(enter);
        }
    }

    private void Update()
    {
        ApplyGravity();
        MovePlayer();
        RotateTowardsMouse();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            if (!isJumping)
            {
                verticalVelocity = -1f;
            }

            isJumping = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void MovePlayer()
    {
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        movement.y = verticalVelocity;

        characterController.Move(movement * Time.deltaTime);
    }

    private void RotateTowardsMouse()
    {
        Vector3 lookDirection = lookTarget - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
