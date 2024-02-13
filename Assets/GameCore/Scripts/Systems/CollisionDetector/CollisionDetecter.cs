using UnityEngine;
using System;

public class CollisionDetecter : MonoBehaviour
{
    [SerializeField] private LayerMask _allowedLayers;
    public Action<Collider, float> OnCollideWithSomething;

    private void OnCollisionEnter(Collision collision)
    {
        if (IsCollisionAllowed(collision.collider))
        {
            float collisionFactor = CalculateCollisionForce(collision);
            OnCollideWithSomething?.Invoke(collision.collider, collisionFactor);
        }
    }

    private bool IsCollisionAllowed(Collider otherCollider) => (_allowedLayers.value & (1 << otherCollider.gameObject.layer)) != 0;

    private float CalculateCollisionForce(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;
        return collisionForce;
    }
}