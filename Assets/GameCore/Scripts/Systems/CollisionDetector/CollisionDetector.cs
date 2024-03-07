using UnityEngine;
using System;

public class CollisionDetector : MonoBehaviour
{
    [SerializeField] private LayerMask _allowedLayers;
    [SerializeField] private bool _debugAllCollision;
    [SerializeField] private bool _useTrigger;

    [SerializeField] private SoundType _collisionSound;

    public Action<Collider, float> OnCollideWithSomething;
    public Action<Collider> OnTriggerE;


    private void Awake()
    {
        OnCollideWithSomething += PlayCollisionSound;
        OnTriggerE += PlayTiggerSound;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsCollisionAllowed(collision.collider))
        {
            float collisionFactor = CalculateCollisionForce(collision);

            if (_debugAllCollision)
                Debug.Log($" {collision.gameObject.name} COLLIDE WITH {gameObject.name}");

            OnCollideWithSomething?.Invoke(collision.collider, collisionFactor);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (_debugAllCollision)
            Debug.Log($" {collider.gameObject.name} COLLIDE WITH {gameObject.name}");

        if (IsCollisionAllowed(collider))
        {
            OnTriggerE?.Invoke(collider);
        }
    }

    private bool IsCollisionAllowed(Collider otherCollider) => (_allowedLayers.value & (1 << otherCollider.gameObject.layer)) != 0;

    private float CalculateCollisionForce(Collision collision)
    {
        float collisionForce = collision.relativeVelocity.magnitude;
        return collisionForce;
    }

    private void PlayCollisionSound(Collider collider, float hitHactor)
    {
        if (_collisionSound != SoundType.none)
            GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(_collisionSound);
    }

    private void PlayTiggerSound(Collider collider)
    {
        if (_collisionSound != SoundType.none)
            GameSoundAndHapticManager.Instance?.PlaySoundAndHaptic(_collisionSound);
    }

    private void OnDestroy()
    {
        if (_collisionSound != SoundType.none)
        {
            OnCollideWithSomething -= PlayCollisionSound;
            OnTriggerE -= PlayTiggerSound;
        }
    }
}