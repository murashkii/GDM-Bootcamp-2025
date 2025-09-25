using UnityEngine;
using System;

public class EnemyHitboxComponent : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 25f;
    
    [Header("Components")]
    [SerializeField] private Collider hitboxCollider;
    
    // Events
    public static event Action<EnemyHitboxComponent> OnEnemyDied;
    public static event Action<EnemyHitboxComponent, float> OnEnemyDamaged;
    
    private bool isDead = false;
    
    private void Awake()
    {
        // Get components
        if (hitboxCollider == null)
            hitboxCollider = GetComponent<Collider>();
        
        // Initialize health
        currentHealth = maxHealth;
        
    }
    
    private void Start()
    {
        // Subscribe to player attack events
        PlayerAttackComponent.OnHitboxActivated += OnPlayerAttackStart;
        PlayerAttackComponent.OnHitboxDeactivated += OnPlayerAttackEnd;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        PlayerAttackComponent.OnHitboxActivated -= OnPlayerAttackStart;
        PlayerAttackComponent.OnHitboxDeactivated -= OnPlayerAttackEnd;
    }
    
    private void OnPlayerAttackStart()
    {
        // Player started attacking - we can now take damage
    }
    
    private void OnPlayerAttackEnd()
    {
        // Player finished attacking
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if we're being hit by player's attack hitbox
        if (other.CompareTag("PlayerHitBox") && !isDead)
        {
            TakeDamage(damageAmount);
        }
    }
    
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        // Trigger damage event
        OnEnemyDamaged?.Invoke(this, damage);
        
        Debug.Log($"Enemy took {damage} damage! Health: {currentHealth}/{maxHealth}");
        
        // Check if enemy died
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        if (isDead) return;
        
        isDead = true;
        
        // Trigger death event
        OnEnemyDied?.Invoke(this);
        
        Debug.Log("Enemy died!");
        
        // Disable components
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
        
        // Disable movement
        EnemyMovementComponent movement = GetComponent<EnemyMovementComponent>();
        if (movement != null)
            movement.enabled = false;
        
        // Disable rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;
        
        // Disable renderer (or play death animation)
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.enabled = false;
        
        // Destroy the game object after a delay
        Destroy(gameObject, 2f);
    }
    
    // Public methods
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = maxHealth;
    }
    
    public void SetDamageAmount(float newDamage)
    {
        damageAmount = newDamage;
    }
    
    public void Heal(float healAmount)
    {
        if (isDead) return;
        
        currentHealth += healAmount;
        currentHealth = Mathf.Min(maxHealth, currentHealth);
        
        Debug.Log($"Enemy healed for {healAmount}! Health: {currentHealth}/{maxHealth}");
    }
    
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
    
    public float GetMaxHealth()
    {
        return maxHealth;
    }
    
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    public bool IsDead()
    {
        return isDead;
    }
    
    public bool IsAlive()
    {
        return !isDead;
    }
}
