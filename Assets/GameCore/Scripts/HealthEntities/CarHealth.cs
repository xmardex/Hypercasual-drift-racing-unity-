using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CollisionDetector))]
public class CarHealth : DamagableEntity
{
    [SerializeField] private HealthBar _healthBar;
    private CollisionDetector _collisionDetecter;

    private void Awake()
    {
        _collisionDetecter = GetComponent<CollisionDetector>();
    }

    private new void Initialize()
    {
        if (_healthBar != null)
        {
            _healthBar.SetHpMinMaxValue(0, MaxHP);
            OnHPChanged += UpdateHealthBar;
            SetCurrentToMaxHP();
        }
        else
        {
            Debug.LogWarning("Entity doesn't have healthBar", this);
        }
    }

    private void SetCurrentToMaxHP()
    {
        _currentHP = MaxHP;
        OnHPChanged?.Invoke(_currentHP);
    }

    private void UpdateHealthBar(float newHP)
    {
        _healthBar?.ChangeValueTo(newHP);
    }

    private void ProcessCarHit(Collider hit, float hitFactor)
    {
        bool collideWithOtherCar = hit.gameObject.TryGetComponent(out CarHealth otherCarHealth);
        bool collideWithProps = hit.gameObject.TryGetComponent(out PropsHealth hittedPropsHealth);

        if (collideWithProps)
            SendDamageTo(hitFactor, hittedPropsHealth);
        else
        {
            //Cars and static obstacle
            ReciveDamageFrom(hitFactor, collideWithOtherCar ? otherCarHealth : null);
        }

        //TODO: Add here other collide scenario

    }

    private void OnDestroy()
    {
        _collisionDetecter.OnCollideWithSomething -= ProcessCarHit;
        OnHPChanged -= UpdateHealthBar;
    }

    public void EnableHealthSystem(bool enable)
    {
        if (enable)
        {
            _collisionDetecter.OnCollideWithSomething += ProcessCarHit;
            _healthBar.ShowHP();

            base.Initialize();
            Initialize();
        }
        else
        {
            _collisionDetecter.OnCollideWithSomething -= ProcessCarHit;
            _healthBar.HideHP();
        }

    }
}
