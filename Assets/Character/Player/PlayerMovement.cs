using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float deceleration = 12f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("Components")]
    [SerializeField] private Rigidbody rb;

    private Vector2 moveInput;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        // Get components
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();

        // Get move action from input system
        if (playerInput != null)
        {
            moveAction = playerInput.actions["Move"];
        }
    }

    private void Start()
    {
        // Set rigidbody settings for 3D top-down movement
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = 0f; // We'll handle deceleration manually
            rb.angularDamping = 0f; // Prevent unwanted rotation damping
            rb.freezeRotation = true; // Prevent physics rotation
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth interpolation
        }
    }

    private void Update()
    {
    }

    private void FixedUpdate()
    {
        HandleInput();

        HandleMovement();
        HandleRotation();

    }

    private void HandleInput()
    {
        if (moveAction != null)
        {
            moveInput = moveAction.ReadValue<Vector2>();
        }
    }

    private void HandleMovement()
    {
        if (rb == null) return;

        // Convert 2D input to 3D movement (X and Z axes only)
        Vector3 horizontalInput = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        // Get current velocity and only modify horizontal components
        Vector3 currentRbVelocity = rb.linearVelocity;
        Vector3 targetHorizontalVelocity = new Vector3(horizontalInput.x, 0, horizontalInput.z);

        // Smooth acceleration/deceleration for horizontal movement only
        if (moveInput.magnitude > 0.1f)
        {
            // Accelerating - smoother response (only on X and Z axes)
            Vector3 currentHorizontalVelocity = new Vector3(currentRbVelocity.x, 0, currentRbVelocity.z);
            Vector3 newHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, acceleration * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentRbVelocity.y, newHorizontalVelocity.z);
        }
        else
        {
            // Decelerating - smoother stop (only on X and Z axes)
            Vector3 currentHorizontalVelocity = new Vector3(currentRbVelocity.x, 0, currentRbVelocity.z);
            Vector3 newHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);

            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentRbVelocity.y, newHorizontalVelocity.z);
        }
    }

    private void HandleRotation()
    {
        // Only rotate if there's significant movement input
        if (moveInput.magnitude > 0.1f)
        {
            // Calculate target rotation based on movement direction
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            // Smooth rotation using Slerp
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }

    public float GetMoveSpeed()
    {
        return moveSpeed;
    }

    public void SetAcceleration(float newAcceleration)
    {
        acceleration = newAcceleration;
    }

    public void SetDeceleration(float newDeceleration)
    {
        deceleration = newDeceleration;
    }

    public void SetRotationSpeed(float newRotationSpeed)
    {
        rotationSpeed = newRotationSpeed;
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }

    public Vector3 GetCurrentVelocity()
    {
        return rb != null ? rb.linearVelocity : Vector3.zero;
    }

    public bool IsMoving()
    {
        return moveInput.magnitude > 0.1f;
    }
    
    public float GetRotationSpeed()
    {
        return rotationSpeed;
    }
}
