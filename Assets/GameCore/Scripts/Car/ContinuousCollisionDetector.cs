using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

//[RequireComponent(typeof(CollisionDetector))]
public class ContinuousCollisionDetector : MonoBehaviour
{
    [SerializeField] private float _collisionDurationMin;
    public Action OnLongTermCollision;
    public Action OnCollisionEnd;

    private bool _longTermCollisionStart = false;

    private Coroutine _collisionWaitIE;

    private List<Collider> _contactColliders = new List<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        OnCollideWithPlayerStart(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnCollideWithPlayerEnd(other);
    }

    private void OnCollideWithPlayerStart(Collider with)
    {
        bool isAI = with.gameObject.CompareTag(Constants.POLICE_CAR_TAG);
        if (isAI)
        {
            CarHealth policeHealth = with.GetComponent<CarHealth>();
            policeHealth.OnDead += RemoveDeadCarsFromList;

            if (_collisionWaitIE == null && !_longTermCollisionStart)
                StartCoroutine(WaitDurationIE(with));
        }
    }

    private void OnCollideWithPlayerEnd(Collider with)
    {
        bool isAI = with.gameObject.CompareTag(Constants.POLICE_CAR_TAG);
        if (isAI)
        {
            CarHealth policeHealth = with.GetComponent<CarHealth>();
            _contactColliders.Remove(with);
            policeHealth.OnDead -= RemoveDeadCarsFromList;

            if (_longTermCollisionStart && _contactColliders.Count == 0)
            {
                OnCollisionEnd?.Invoke();
                _longTermCollisionStart = false;
            }
        }
    }

    private void RemoveDeadCarsFromList()
    {
        foreach(Collider collider in _contactColliders.ToArray())
        {
            if (collider.GetComponent<CarHealth>().IsDead)
                _contactColliders.Remove(collider);
        }
        if(_contactColliders.Count == 0)
        {
            OnCollisionEnd?.Invoke();
            _longTermCollisionStart = false;
        }
    }

    IEnumerator WaitDurationIE(Collider with)
    {
        yield return new WaitForSeconds(_collisionDurationMin);
        OnLongTermCollision?.Invoke();
        _contactColliders.Add(with);
        _longTermCollisionStart = true;
    }
}