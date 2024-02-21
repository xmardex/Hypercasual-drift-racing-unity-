using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CollisionDetector))]
public class CarHealth : DamagableEntity
{
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private List<HealthFXStates> _healthFXStates;

    private HealthFXStates _currentHealthFXState;
    private CollisionDetector _collisionDetecter;

    public Action OnHit;

    private void Awake()
    {
        _collisionDetecter = GetComponent<CollisionDetector>();
    }

    private new void Initialize()
    {
        if (_healthBar != null)
        {
            _healthBar.SetHpMinMaxValue(0, MaxHP);
            OnDead += _healthBar.HideHP;
            OnHPChanged += UpdateHealthBar;
            SetCurrentToMaxHP();
        }

        OnDead += DisableAllStatesFX;
    }

    private void SetCurrentToMaxHP()
    {
        _currentHP = MaxHP;
        OnHPChanged?.Invoke(_currentHP);
    }

    private void UpdateHealthBar(float newHP)
    {
        _healthBar?.ChangeValueTo(newHP);
        ApplyHealthStateFX();
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

        OnHit?.Invoke();
    }

    private void OnDestroy()
    {
        _collisionDetecter.OnCollideWithSomething -= ProcessCarHit;
        if (_healthBar != null)
        {
            OnHPChanged -= UpdateHealthBar;
            OnDead -= _healthBar.HideHP;
        }
        OnDead -= DisableAllStatesFX;
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

    private void ApplyHealthStateFX()
    {
        float currentProportion = CurrentHP / MaxHP;

        if (_healthFXStates != null && _healthFXStates.Count > 0)
        {
            HealthFXStates healthFXStates = null;

            foreach (var healthState in _healthFXStates)
            {
                if (currentProportion >= healthState.minProportion &&
                    currentProportion <= healthState.maxProportion)
                {
                    healthFXStates = healthState;
                    break;
                }
            }

            if (healthFXStates != null)
            {
                _currentHealthFXState?.fx?.Stop();
                healthFXStates.fx.gameObject.SetActive(true);
                healthFXStates.fx.Play();
               _currentHealthFXState = healthFXStates;
            }
            else
            {
                _currentHealthFXState?.fx?.Stop();
            }
        }
    }
    private void DisableAllStatesFX()
    {
        foreach (var healthState in _healthFXStates)
        {
            healthState.fx.Stop();
        }
    }
}

[Serializable]
public class HealthFXStates
{
    [Range(0f, 1f)]
    public float minProportion;
    [Range(0f, 1f)]
    public float maxProportion;
    public ParticleSystem fx;
}