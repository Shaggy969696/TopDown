using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 20f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDecay = 8f;  // velocidad con que se frena el empuje

    private Camera mainCamera;
    private CharacterController characterController;
    private PlayerAnima playerAnima;
    private Vector2 moveInput;
    private Vector3 lookTarget;
    private float verticalVelocity;
    private bool isJumping;

    private Vector3 knockbackVelocity = Vector3.zero;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        // Usar GetComponentInChildren por si pusiste el script PlayerAnima en el objeto T_Pose en vez de en el principal
        playerAnima = GetComponentInChildren<PlayerAnima>();
        mainCamera = Camera.main;

        if (playerAnima == null)
        {
            Debug.LogWarning("PlayerController: No se encontró el script PlayerAnima ni en este objeto ni en sus hijos.");
        }
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

    /// <summary>
    /// Aplica un empuje al jugador en la dirección indicada.
    /// </summary>
    public void ApplyKnockback(Vector3 direction, float force)
    {
        knockbackVelocity = direction.normalized * force;
    }

    private void Update()
    {
        ApplyGravity();
        MovePlayer();
        RotateTowardsMouse();

        if (playerAnima != null)
        {
            playerAnima.SetRunning(moveInput.sqrMagnitude > 0.01f);
        }
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded)
        {
            if (!isJumping) verticalVelocity = -1f;
            isJumping = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void MovePlayer()
    {
        // Movimiento normal
        Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed;
        movement.y = verticalVelocity;

        // Aplicar y atenuar knockback
        movement += knockbackVelocity;
        knockbackVelocity = Vector3.Lerp(knockbackVelocity, Vector3.zero, knockbackDecay * Time.deltaTime);

        characterController.Move(movement * Time.deltaTime);
    }

    private void RotateTowardsMouse()
    {
        Vector3 lookDirection = lookTarget - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude <= 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
