using UnityEngine;
using System;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _allowedLayers;
    [SerializeField] private bool _debugAllCollision;
    [SerializeField] private bool _useTrigger;
    public Action<Collider, float> OnCollideWithSomething;



    private void OnCollisionEnter(Collision collision)
    {
        if (IsCollisionAllowed(collision.collider))
        {
            float collisionFactor = CalculateCollisionForce(collision);
            OnCollideWithSomething?.Invoke(collision.collider, collisionFactor);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_debugAllCollision)
            Debug.Log(collider.gameObject.name);

        if (IsCollisionAllowed(collider))
        {
            float collisionFactor = 0;
            OnCollideWithSomething?.Invoke(collider, collisionFactor);
        }
    }

    private bool IsCollisionAllowed(Collider otherCollider) => (_allowedLayers.value & (1 << otherCollider.gameObject.layer)) != 0;

    private float CalculateCollisionForce(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;
        return collisionForce;
    }
}