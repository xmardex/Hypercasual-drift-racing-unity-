using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDeath : MonoBehaviour
{
    [SerializeField] private ParticleSystem _explodeFX;
    [SerializeField] private float _explodeForce;
    [SerializeField] private float _explodeRadius;
    [SerializeField] private float _upwardsModifier = 3f;
    [SerializeField] private GameObject[] _aliveCarObjects;
    [SerializeField] private GameObject[] _deadCarObjects;

    [SerializeField] private Rigidbody _bodyAlive;
    [SerializeField] private Rigidbody _bodyDead;

    [SerializeField] private Rigidbody[] _aliveCarWheels;
    [SerializeField] private Rigidbody[] _deadCarWheels;

    [SerializeField] private CarReferences _carReferences;

    private CapsuleCollider _carCollider;

    private CarAI _carAI;

    private bool _isExploded;
    private bool _useHP;
    private bool _isAI;

    public void Initialize(bool useHP)
    {
        _useHP = useHP;
        _isExploded = false;

        _isAI = TryGetComponent(out _carAI);

        _carCollider = GetComponent<CapsuleCollider>();
        if (_isAI || _useHP)
        {
            _carReferences.CarHealth.OnDead += DisableCarMovement;
            _carReferences.CarHealth.OnDead += ExplodeCar;
        }
    }

    public void ExplodeCar()
    {
        if (!_isExploded)
        {
            //play fx should be "play on awake"=true and loop=false
            if (_explodeFX != null)
            {
                _explodeFX.gameObject.SetActive(true);
                _explodeFX.Play();
            }

                _carCollider.enabled = false;
            // flip car parts to dead one
            for (int i = 0; i < _aliveCarObjects.Length; i++)
            {
                GameObject aliveCarMesh = _aliveCarObjects[i];
                GameObject deadCarMesh = _deadCarObjects[i];
                aliveCarMesh.SetActive(false);
                deadCarMesh.SetActive(true);
                deadCarMesh.transform.SetParent(null);
            }
            for (int i = 0; i < _aliveCarWheels.Length; i++)
            {
                Rigidbody wheelAlive = _aliveCarWheels[i];
                wheelAlive.gameObject.SetActive(false);
                Rigidbody wheelDead = _deadCarWheels[i];
                wheelDead.velocity = wheelAlive.velocity;
                wheelDead.angularVelocity = wheelAlive.angularVelocity;
            }
            _bodyDead.velocity = _bodyAlive.velocity;
            _bodyDead.angularVelocity = _bodyAlive.angularVelocity;

            //TODO: Camera on death here or else;
            _bodyAlive.isKinematic = true;

            // add explode force 
            Explode(transform.position);
            _isExploded = true;
        }

        _carAI?.DeactivateAI();
    }

    private void Explode(Vector3 explodePosition)
    {
        Collider[] colliders = Physics.OverlapSphere(explodePosition, _explodeRadius);
        foreach (Collider hit in colliders)
        {
            if(hit.TryGetComponent(out Rigidbody rb))
            {
                rb.AddExplosionForce(_explodeForce, explodePosition, _explodeRadius, _upwardsModifier);
                //TODO: ADD HERE 
                if(hit.TryGetComponent(out TrafficCarController trafficCar))
                {
                    trafficCar.StopMoving();
                }
                //if(hit.TryGetComponent(out CarAI carAI))
                //{
                //    carAI.DeactivateAI();
                //}
                if(hit.TryGetComponent(out DamagableEntity damagableEntity))
                {
                    if(damagableEntity != _carReferences.CarHealth && !damagableEntity.IsDead)
                    {
                        damagableEntity.Damage(damagableEntity.MaxHP/Constants.ON_EXPLODE_DAMAGE_MAX_DAMAGE_DIVIDER);
                    }
                }
                //stop traffic near
                //stop police
                //kill everithing that have health
            }
        }
    }

    private void DisableCarMovement()
    {
        _carReferences.CarController.SetCanMove(false);
    }

    private void OnDestroy()
    {
        if (_isAI || _useHP)
        {
            _carReferences.CarHealth.OnDead -= ExplodeCar;
            _carReferences.CarHealth.OnDead -= DisableCarMovement;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explodeRadius);
    }
}
