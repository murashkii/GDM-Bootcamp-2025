using UnityEngine;

public class EnemyMovementComponent : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float acceleration = 6f;
    [SerializeField] private float deceleration = 8f;
    
    [Header("Components")]
    [SerializeField] private Rigidbody rb;
    
    private Transform playerTransform;
    private bool isFollowingPlayer = false;
    
    private void Awake()
    {
        // Get components
        if (rb == null)
            rb = GetComponent<Rigidbody>();
    }
    
    private void Start()
    {
        // Get player transform from PlayerManager singleton
        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerMovement != null)
        {
            playerTransform = PlayerManager.Instance.PlayerMovement.transform;
        }
        
        // Set rigidbody settings
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f; // Prevent unwanted rotation damping
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth interpolation
        }
    }
    
    private void Update()
    {
        if (playerTransform == null)
        {
            // Try to get player transform again if it wasn't available at Start
            if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerMovement != null)
            {
                playerTransform = PlayerManager.Instance.PlayerMovement.transform;
            }
            return;
        }
        
        CheckPlayerDistance();
        HandleMovement();
        HandleRotation();
    }
    
    private void CheckPlayerDistance()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceToPlayer <= detectionRange)
        {
            isFollowingPlayer = true;
        }
        else
        {
            isFollowingPlayer = false;
        }
    }
    
    private void HandleMovement()
    {
        if (!isFollowingPlayer || rb == null) return;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Get current velocity and only modify horizontal components
        Vector3 currentRbVelocity = rb.linearVelocity;
        
        // Stop moving if too close to player
        if (distanceToPlayer <= stopDistance)
        {
            // Decelerate to stop (only on X and Z axes)
            Vector3 currentHorizontalVelocity = new Vector3(currentRbVelocity.x, 0, currentRbVelocity.z);
            Vector3 newHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, Vector3.zero, deceleration * Time.fixedDeltaTime);
            
            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentRbVelocity.y, newHorizontalVelocity.z);
        }
        else
        {
            // Move towards player (only on X and Z axes, preserve Y for gravity)
            Vector3 horizontalDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z).normalized;
            Vector3 targetHorizontalVelocity = horizontalDirection * moveSpeed;
            
            // Smooth movement (only on X and Z axes)
            Vector3 currentHorizontalVelocity = new Vector3(currentRbVelocity.x, 0, currentRbVelocity.z);
            Vector3 newHorizontalVelocity = Vector3.Lerp(currentHorizontalVelocity, targetHorizontalVelocity, acceleration * Time.fixedDeltaTime);
            
            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, currentRbVelocity.y, newHorizontalVelocity.z);
        }
    }
    
    private void HandleRotation()
    {
        if (!isFollowingPlayer) return;
        
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 horizontalDirection = new Vector3(directionToPlayer.x, 0, directionToPlayer.z);
        
        if (horizontalDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    // Public methods
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    
    public void SetDetectionRange(float newRange)
    {
        detectionRange = newRange;
    }
    
    public void SetStopDistance(float newDistance)
    {
        stopDistance = newDistance;
    }
    
    public bool IsFollowingPlayer()
    {
        return isFollowingPlayer;
    }
    
    public float GetDistanceToPlayer()
    {
        if (playerTransform == null) return float.MaxValue;
        return Vector3.Distance(transform.position, playerTransform.position);
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        // Draw detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Draw stop distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
