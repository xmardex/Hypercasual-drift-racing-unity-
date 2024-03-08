using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CollisionDetector))]
public class ContinuousCollisionDetector : MonoBehaviour
{
    [SerializeField] private float _collisionDurationMin;
    private CollisionDetector _collisionDetector;
    private bool _collide = false;

    public Action<Collider> OnLongTermCollision;
    public Action<Collider> OnCollisionEnd;

    private bool _longTermCollisionStart = false;

    private Coroutine _collisionWaitIE;

    private List<Collider> _contactColliders = null;


    private void Awake()
    {
        _contactColliders ??= new List<Collider>();
        _collisionDetector = GetComponent<CollisionDetector>();
        _collisionDetector.OnCollideWithSomething += OnCollideWithPlayerStart;
        _collisionDetector.OnCollideEndWithSomething += OnCollideWithPlayerEnd;
    }

    private void OnCollideWithPlayerStart(Collider with, float force)
    {
        bool isAI = with.gameObject.CompareTag(Constants.POLICE_CAR_TAG) || with.gameObject.CompareTag(Constants.POLICE_CAR_ELEMENTS_TAG);
        if (isAI)
        {
            _collide = true;
            if(_collisionWaitIE != null)
                StopCoroutine(_collisionWaitIE);
            _collisionWaitIE = StartCoroutine(WaitDurationIE(with));
        }
    }

    private void OnCollideWithPlayerEnd(Collider with)
    {
        bool isAI = with.gameObject.CompareTag(Constants.POLICE_CAR_TAG) || with.gameObject.CompareTag(Constants.POLICE_CAR_ELEMENTS_TAG);
        if (isAI)
        {
            _collide = false;
            if (_collisionWaitIE != null)
                StopCoroutine(_collisionWaitIE);
            if (_longTermCollisionStart)
            {
                OnCollisionEnd?.Invoke(with);
                _longTermCollisionStart = false;
            }
        }
    }

    private void OnDestroy()
    {
        _collisionDetector.OnCollideWithSomething -= OnCollideWithPlayerStart;
        _collisionDetector.OnCollideEndWithSomething -= OnCollideWithPlayerEnd;
    }

    IEnumerator WaitDurationIE(Collider with)
    {
        yield return new WaitForSeconds(_collisionDurationMin);
        if (_collide)
        {
            OnLongTermCollision?.Invoke(with);
            _longTermCollisionStart = true;
        }
    }
}
