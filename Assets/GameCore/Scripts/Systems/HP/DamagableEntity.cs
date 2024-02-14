using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DamagableEntity : MonoBehaviour, IDamageDealer, IHealth
{
    [SerializeField] private HealthSO _healthSO;
    protected float _maxHP;
    protected float _currentHP;

    protected float _minDamage;
    protected float _maxDamage;

    //Factor value is (abs of two velocity.magnitude of cars) representing hit force; 
    protected float _minDamageFactorValue; //if minDamageFactorValue is reached - deal minDamage;
    protected float _maxDamageFactorValue; //if maxDamageFactorValue is reached or more - deal maxDamage;

    public Action<float> OnHPChanged;
    public Action<bool> OnDead;
    public Action<IDamageDealer, float> OnDamageRecive;


    public float MaxHP => _maxHP;
    public float CurrentHP => _currentHP;
    public float MinDamage => _minDamage;
    public float MaxDamage => _maxDamage;
    public float MinDamageFactorValue => _maxDamageFactorValue;
    public float MaxDamageFactorValue => _maxDamageFactorValue;

    protected virtual void Initialize()
    {
        ApplyPreset();
    }

    protected void ApplyPreset()
    {
        if (_healthSO != null)
        {
            _maxHP = _healthSO.MaxHP;
            _minDamage = _healthSO.MinDamage;
            _maxDamage = _healthSO.MaxDamage;
            _minDamageFactorValue = _healthSO.MinDamageFactorValue;
            _maxDamageFactorValue = _healthSO.MaxDamageFactorValue;
        }
        else
        {
            Debug.LogError("Car health SO doesnt set", this);
        }
    }

    public virtual void ReciveDamageFrom(float damageFactor, IDamageDealer from)
    {
        Damage(CalculateActualDamage(damageFactor));
        OnDamageRecive?.Invoke(from, damageFactor);
    }

    public virtual void SendDamageTo(float damageFactor, IHealth to)
    {
        to.ReciveDamageFrom(damageFactor, this);
    }

    public virtual float CalculateActualDamage(float damageFactor)
    {
        float actualDamage = 0f;

        if (damageFactor < _minDamageFactorValue)
        {
            actualDamage = 0f;
        }
        else if (damageFactor >= _minDamageFactorValue && damageFactor <= _maxDamageFactorValue)
        {
            float factorRange = _maxDamageFactorValue - _minDamageFactorValue;
            float damageRange = _maxDamage;

            float factorRelativeToRange = (damageFactor - _minDamageFactorValue) / factorRange;
            actualDamage = factorRelativeToRange * damageRange;
        }
        else
        {
            actualDamage = _maxDamage;
        }
        return actualDamage;
    }


    public virtual void Heal(float heal)
    {
        _currentHP = (CurrentHP + heal) > MaxHP ? MaxHP : (_currentHP + heal);

        OnHPChanged?.Invoke(_currentHP);
    }

    public virtual void Damage(float damage)
    {
        _currentHP = (CurrentHP - damage) < 0 ? 0 : (_currentHP - damage);

        OnHPChanged?.Invoke(_currentHP);
        if (_currentHP == 0)
            OnDead?.Invoke(true);
    }


}
