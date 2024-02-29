using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ExplodeBase : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _eplosionFXs;
    [SerializeField] private bool _deatachFX = true;

    [SerializeField] private GameObject[] _aliveObjects;
    [SerializeField] private GameObject[] _deadObjecrs;
    [SerializeField] private Rigidbody[] _rbsToActivate;

    [SerializeField] private bool _useExplosionForce;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private float _explisionForce;
    [SerializeField] private float _explosionUpwardModifier;
    [SerializeField] private Vector3 _explosionPositionOffset;

    protected void Explode(DamagableEntity self, out List<DamagableEntity> damagableEntitiesAffected)
    {
        Vector3 actualExplosionPosition = transform.position + _explosionPositionOffset;
        damagableEntitiesAffected = new List<DamagableEntity>();



        foreach (GameObject obj in _aliveObjects)
        {
            obj.SetActive(false);
        }

        foreach (GameObject obj in _deadObjecrs)
        {
            obj.transform.SetParent(null);
            obj.SetActive(true);
        }

        foreach (ParticleSystem ep in _eplosionFXs)
        {
            if(_deatachFX)
                ep.gameObject.transform.SetParent(null);
            ep.gameObject.SetActive(true);
            ep.Play();
        }

        foreach (Rigidbody rb in _rbsToActivate)
        {
            rb.isKinematic = false;
        }

        Collider[] colliders = Physics.OverlapSphere(actualExplosionPosition, _explosionRadius);
        foreach (Collider hit in colliders)
        {
            if (hit.TryGetComponent(out Rigidbody rb))
            {
                if(!hit.GetComponent<ConfigurableJoint>())
                    rb.AddExplosionForce(_explisionForce, actualExplosionPosition, _explosionRadius, _explosionUpwardModifier);
            }
            DamagableEntity damagableEntity = hit.GetComponentInParent<DamagableEntity>();
            if (damagableEntity != null && damagableEntity != self && !damagableEntitiesAffected.Contains(damagableEntity))
            {
                damagableEntitiesAffected.Add(damagableEntity);
            }
        }

        gameObject.SetActive(false);
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 actualExplosionPosition = transform.position + _explosionPositionOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(actualExplosionPosition, _explosionRadius);
    }

}
