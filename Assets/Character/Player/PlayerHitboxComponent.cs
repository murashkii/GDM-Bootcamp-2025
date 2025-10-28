using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitboxComponent : MonoBehaviour
{
    [SerializeField] private Collider _hurtBox;
    [SerializeField] private float _maxHealth = 100;

    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount)
    {
        if (damageAmount <= 0)
        {
            //error less than 0
            return;
        }

        _currentHealth -= damageAmount;

        DeathCheck();
    }

    private void DeathCheck()
    {
        if (_currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
