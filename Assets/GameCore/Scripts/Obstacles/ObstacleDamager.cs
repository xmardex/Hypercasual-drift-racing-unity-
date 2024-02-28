using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleDamager : MonoBehaviour
{
    [SerializeField] private CollisionDetector _collisionDetector;
    [SerializeField] private float _damageValue;
    [SerializeField] private bool _killTarget;
    private void Awake()
    {
        _collisionDetector.OnCollideWithSomething += DealDamage;
    }

    public void DealDamage(Collider hitBy, float withFactor)
    {
        DamagableEntity damagableEntity = hitBy.GetComponentInParent<DamagableEntity>();
        if (damagableEntity != null && !damagableEntity.IsDead)
        {
            damagableEntity.Damage(_killTarget ? damagableEntity.MaxHP : _damageValue);
        }
    }
}
