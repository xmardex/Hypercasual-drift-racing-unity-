using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObstacle : MonoBehaviour
{
    [SerializeField] private CollisionDetector[] _collisionDetectors;
    [SerializeField] private Rigidbody[] _rigidbodiesToActivate;
    [SerializeField] private GameObject[] _supportsToDestroy;
    [SerializeField] private ParticleSystem[] _destroySupportFXs;

    private bool _isDestructed;

    private void Awake()
    {
        foreach(CollisionDetector collisionDetector in _collisionDetectors) 
        {
            collisionDetector.OnTriggerE += DestructObstacle;
        }
    }

    private void DestructObstacle(Collider hitBy)
    {
        if (!_isDestructed)
        {
            _isDestructed = true;

            foreach (CollisionDetector collisionDetector in _collisionDetectors)
                collisionDetector.OnTriggerE -= DestructObstacle;

            foreach(var fx in _destroySupportFXs)
            {
                fx.gameObject.transform.SetParent(null);
                fx.gameObject.SetActive(true);
                fx.Play();
            }

            foreach (var obj in _supportsToDestroy)
                Destroy(obj);

            foreach (var rb in _rigidbodiesToActivate)
            {
                rb.transform.SetParent(null);
                rb.isKinematic = false;
            }
        }
    }
}
