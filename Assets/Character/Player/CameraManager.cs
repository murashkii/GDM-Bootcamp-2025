using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    [Header("Camera Settings")]
    [SerializeField] private float followSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);
    [SerializeField] private float lookAtHeight = 1f;
    
    [Header("Components")]
    [SerializeField] private Camera cam;
    
    private Transform playerTransform;
    private Vector3 targetPosition;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Get camera component if not assigned
        if (cam == null)
            cam = GetComponent<Camera>();
    }
    
    private void Start()
    {
        // Get player transform from PlayerManager singleton
        if (PlayerManager.Instance != null && PlayerManager.Instance.PlayerMovement != null)
        {
            playerTransform = PlayerManager.Instance.PlayerMovement.transform;
        }
    }
    
    private void LateUpdate()
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
        
        FollowPlayer();
    }
    
    private void FollowPlayer()
    {
        // Calculate target position with 45-degree angle offset
        Vector3 playerPosition = playerTransform.position;
        
        // Apply offset relative to player position
        targetPosition = playerPosition + offset;
        
        // Smoothly move camera to target position
        transform.position = Vector3.Slerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        
        // Look at player with slight height offset for better view
        Vector3 lookAtTarget = playerPosition + Vector3.up * lookAtHeight;
        Vector3 lookDirection = lookAtTarget - transform.position;
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }
    
    public void SetFollowSpeed(float newSpeed)
    {
        followSpeed = newSpeed;
    }
    
    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }
    
    public void SetLookAtHeight(float newHeight)
    {
        lookAtHeight = newHeight;
    }
    
    public Vector3 GetCurrentOffset()
    {
        return offset;
    }
    
    public float GetFollowSpeed()
    {
        return followSpeed;
    }
}
