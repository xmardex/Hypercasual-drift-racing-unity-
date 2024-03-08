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

            _contactColliders.Add(with);
            Debug.Log("INVOKE");
            OnLongTermCollision?.Invoke();
        }
    }

    private void OnCollideWithPlayerEnd(Collider with)
    {
        bool isAI = with.gameObject.CompareTag(Constants.POLICE_CAR_TAG);
        if (isAI)
        {
            CarHealth policeHealth = with.GetComponent<CarHealth>();
            policeHealth.OnDead -= RemoveDeadCarsFromList;

            _contactColliders.Remove(with);

            if(_contactColliders.Count == 0)
                OnCollisionEnd?.Invoke();
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
        }
    }
}