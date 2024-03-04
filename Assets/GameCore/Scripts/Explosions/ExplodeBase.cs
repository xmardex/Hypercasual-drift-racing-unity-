using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ExplodeBase : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _eplosionFXs;
    [SerializeField] private bool _deatachFX = true;

    [SerializeField] private GameObject[] _aliveObjects;
    [SerializeField] private GameObject[] _deadObjects;
    [SerializeField] private Rigidbody[] _rbsToActivate;

    [SerializeField] private bool _useExplosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explisionForce;
    [SerializeField] private float _explosionUpwardModifier;
    [SerializeField] private Vector3 _explosionPositionOffset;

    private bool _objectsReplaced = false;
    private Vector3 actualExplosionPosition;

    protected void Explode(DamagableEntity self, out List<DamagableEntity> damagableEntitiesAffected, out List<Rigidbody> explosionAffectedRBs)
    {
        actualExplosionPosition = transform.position + _explosionPositionOffset;

        SwitchToDead();

        damagableEntitiesAffected = new List<DamagableEntity>();
        explosionAffectedRBs = new List<Rigidbody>();

        Collider[] colliders = Physics.OverlapSphere(actualExplosionPosition, _explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Rigidbody rb))
            {
                if(!hit.GetComponent<ConfigurableJoint>())
                    explosionAffectedRBs.Add(rb);
                    
            }
            DamagableEntity damagableEntity = hit.GetComponentInParent<DamagableEntity>();
            if (damagableEntity != null && damagableEntity != self && !damagableEntitiesAffected.Contains(damagableEntity))
            {
                damagableEntitiesAffected.Add(damagableEntity);
            }
        }
    }

    protected void SwitchToDead()
    {
        foreach (GameObject obj in _aliveObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in _deadObjects)
        {
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }

        foreach (ParticleSystem ep in _eplosionFXs)
        {
            if (_deatachFX)
                ep.gameObject.transform.SetParent(null);
            ep.gameObject.SetActive(true);
            ep.Play();
        }

        foreach (Rigidbody rb in _rbsToActivate)
        {
            rb.isKinematic = false;
        }
    }

    protected void ApplyExplosionForces(List<Rigidbody> explosionAffectedRBs)
    {
        foreach (var rb in explosionAffectedRBs)
        {
            rb.AddExplosionForce(_explisionForce, actualExplosionPosition, _explosionRadius, _explosionUpwardModifier);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 actualExplosionPosition = transform.position + _explosionPositionOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(actualExplosionPosition, _explosionRadius);
    }

}
