using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    // Static counter for automatic ID generation
    private static int nextEnemyID = 1;
    
    [Header("Enemy Components")]
    [SerializeField] private EnemyMovementComponent movementComponent;
    [SerializeField] private EnemyHitboxComponent hitboxComponent;
    [SerializeField] private EnemyAttackComponent attackComponent;
    
    [Header("Enemy Settings")]
    [SerializeField] private string enemyName = "Enemy";
    [SerializeField] private int enemyID;
    [SerializeField] private bool isActive = true;
    
    // Properties
    public EnemyMovementComponent Movement => movementComponent;
    public EnemyHitboxComponent Hitbox => hitboxComponent;
    public string EnemyName => enemyName;
    public int EnemyID => enemyID;
    public bool IsActive => isActive;
    public bool IsAlive => hitboxComponent != null && hitboxComponent.IsAlive();
    public bool IsDead => hitboxComponent != null && hitboxComponent.IsDead();
    
    private void Awake()
    {
        // Assign unique ID using static counter
        enemyID = nextEnemyID++;
        
        // Get components if not assigned
        if (movementComponent == null)
            movementComponent = GetComponent<EnemyMovementComponent>();
            
        if (hitboxComponent == null)
            hitboxComponent = GetComponent<EnemyHitboxComponent>();

        attackComponent.SetMovementComponent(movementComponent);
    }
    
    private void Start()
    {
        // Subscribe to hitbox events
        if (hitboxComponent != null)
        {
            EnemyHitboxComponent.OnEnemyDied += OnEnemyDied;
            EnemyHitboxComponent.OnEnemyDamaged += OnEnemyDamaged;
        }
        
        // Initialize enemy
        InitializeEnemy();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (hitboxComponent != null)
        {
            EnemyHitboxComponent.OnEnemyDied -= OnEnemyDied;
            EnemyHitboxComponent.OnEnemyDamaged -= OnEnemyDamaged;
        }
    }
    
    private void InitializeEnemy()
    {
        // Set enemy as active
        SetActive(true);
        
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) initialized!");
    }
    
    private void OnEnemyDied(EnemyHitboxComponent deadEnemy)
    {
        // Check if this is our enemy that died
        if (deadEnemy == hitboxComponent)
        {
            HandleDeath();
        }
    }
    
    private void OnEnemyDamaged(EnemyHitboxComponent damagedEnemy, float damage)
    {
        // Check if this is our enemy that took damage
        if (damagedEnemy == hitboxComponent)
        {
            HandleDamage(damage);
        }
    }
    
    private void HandleDeath()
    {
        isActive = false;
        
        // Disable movement
        if (movementComponent != null)
        {
            movementComponent.enabled = false;
        }
        
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) has died!");
    }
    
    private void HandleDamage(float damage)
    {
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) took {damage} damage!");
    }
    
    // Public methods
    public void SetActive(bool active)
    {
        isActive = active;
        
        if (movementComponent != null)
            movementComponent.enabled = active;
            
        if (hitboxComponent != null)
            hitboxComponent.enabled = active;
    }
    
    public void SetEnemyName(string newName)
    {
        enemyName = newName;
    }
    
    public void SetEnemyID(int newID)
    {
        enemyID = newID;
    }
    
    public void TakeDamage(float damage)
    {
        if (hitboxComponent != null && IsAlive)
        {
            hitboxComponent.TakeDamage(damage);
        }
    }
    
    public void Heal(float healAmount)
    {
        if (hitboxComponent != null && IsAlive)
        {
            hitboxComponent.Heal(healAmount);
        }
    }
    
    public void SetMoveSpeed(float speed)
    {
        if (movementComponent != null)
        {
            movementComponent.SetMoveSpeed(speed);
        }
    }
    
    public void SetDetectionRange(float range)
    {
        if (movementComponent != null)
        {
            movementComponent.SetDetectionRange(range);
        }
    }
    
    public void SetStopDistance(float distance)
    {
        if (movementComponent != null)
        {
            movementComponent.SetStopDistance(distance);
        }
    }
    
    public void SetMaxHealth(float maxHealth)
    {
        if (hitboxComponent != null)
        {
            hitboxComponent.SetMaxHealth(maxHealth);
        }
    }
    
    public void SetDamageAmount(float damage)
    {
        if (hitboxComponent != null)
        {
            hitboxComponent.SetDamageAmount(damage);
        }
    }
    
    // Getters
    public float GetCurrentHealth()
    {
        return hitboxComponent != null ? hitboxComponent.GetCurrentHealth() : 0f;
    }
    
    public float GetMaxHealth()
    {
        return hitboxComponent != null ? hitboxComponent.GetMaxHealth() : 0f;
    }
    
    public float GetHealthPercentage()
    {
        return hitboxComponent != null ? hitboxComponent.GetHealthPercentage() : 0f;
    }
    
    public float GetDistanceToPlayer()
    {
        return movementComponent != null ? movementComponent.GetDistanceToPlayer() : float.MaxValue;
    }
    
    public bool IsFollowingPlayer()
    {
        return movementComponent != null ? movementComponent.IsFollowingPlayer() : false;
    }
    
    // Utility methods
    public void ResetEnemy()
    {
        if (hitboxComponent != null)
        {
            hitboxComponent.SetMaxHealth(hitboxComponent.GetMaxHealth());
        }
        
        SetActive(true);
        
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) has been reset!");
    }
    
    public void DisableEnemy()
    {
        SetActive(false);
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) has been disabled!");
    }
    
    public void EnableEnemy()
    {
        SetActive(true);
        Debug.Log($"Enemy '{enemyName}' (ID: {enemyID}) has been enabled!");
    }
    
    // Static methods for ID counter management
    public static int GetNextEnemyID()
    {
        return nextEnemyID;
    }
    
    public static void ResetEnemyIDCounter()
    {
        nextEnemyID = 1;
        Debug.Log("Enemy ID counter has been reset to 1");
    }
    
    public static int GetTotalEnemiesCreated()
    {
        return nextEnemyID - 1;
    }
}
