using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsHealth : DamagableEntity
{
    [SerializeField] private CollisionDetector _collisionDetector;

    private PropsExplode _explode;

    private bool _isExploded;

    private void Start()
    {
        Initialize();
    }
    private new void Initialize()
    {
        base.Initialize();
        _explode = GetComponent<PropsExplode>();
        _currentHP = MaxHP;
        _collisionDetector.OnCollideWithSomething += PropsHit;
    }

    private void PropsHit(Collider hitBy, float damageFactor)
    {
        //Debug.Log($"PROPS {gameObject.name} hit by {hitBy.gameObject.name} with factor {damageFactor}");

        ReciveDamageFactor(damageFactor);

        if (_currentHP == 0 && _explode != null && !_isExploded)
        {
            _explode.Explode();
            _isExploded = true;
        }
    }

    private void OnDestroy()
    {
        if(_collisionDetector !=null)
            _collisionDetector.OnCollideWithSomething -= PropsHit;
    }
}
