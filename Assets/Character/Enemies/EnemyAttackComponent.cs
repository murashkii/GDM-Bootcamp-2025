using UnityEngine;

public class EnemyAttackComponent : MonoBehaviour
{
    [SerializeField] private float _damage = 10;

    private EnemyMovementComponent _movementComponent;
    public void SetMovementComponent (EnemyMovementComponent component)
    {
        _movementComponent = component;

    }

    public void DealDamage()
    {
        PlayerManager.Instance.hitbox.TakeDamage(_damage);
    }
   
}
