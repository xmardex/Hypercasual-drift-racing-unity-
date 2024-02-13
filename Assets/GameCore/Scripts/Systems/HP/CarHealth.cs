using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionDetecter))]
public class CarHealth : DamagableEntity
{
    [SerializeField] private CollisionDetecter _collisionDetecter;
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _collisionDetecter = GetComponent<CollisionDetecter>();
        _collisionDetecter.OnCollideWithSomething += ProcessCarHit;
        SetCurrentHP();
    }

    private void SetCurrentHP()
    {
        _currentHP = _maxHP;
    }

    private void ProcessCarHit(Collider hit, float hitFactor)
    {
        bool collideWithOtherCar = hit.gameObject.TryGetComponent(out CarHealth otherCarHealth);
            ReciveDamageFrom(hitFactor, collideWithOtherCar ? otherCarHealth : null);
    }

    private void OnDestroy()
    {
        _collisionDetecter.OnCollideWithSomething -= ProcessCarHit;
    }

}
