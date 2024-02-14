using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsHealth : DamagableEntity
{
    [SerializeField] private GameObject _destroyFX;

    private bool _isDestroyed;

    private void Start()
    {
        Initialize();
    }
    private new void Initialize()
    {
        base.Initialize();
        _currentHP = MaxHP;
        OnDamageRecive += PropsHit;
    }

    private void PropsHit(IDamageDealer damageDealer, float damageFactor)
    {
        if (_currentHP == 0)
            PropsDestroy();
        if (damageDealer is IHealth damagable)
            damageDealer.SendDamageTo(damageFactor, damagable);
    }

    private void PropsDestroy()
    {
        _isDestroyed = true;
        _destroyFX.transform.SetParent(null);
        _destroyFX.SetActive(true);
        Destroy(gameObject);
        //explode
    }

    private void OnDestroy()
    {
        OnDamageRecive -= PropsHit;
    }
}
