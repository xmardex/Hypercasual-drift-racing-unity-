using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNitro : MonoBehaviour
{
    [SerializeField] private float _duration;
    [SerializeField] private float _nitroSpeedUpValue;
    [SerializeField] private float _frictionOnNitro;
    [SerializeField] private float _brakeVelocityLimit;

    [SerializeField] private ParticleSystem[] _nitroFXs;

    private CarController _carController;

    private Coroutine _nitroIE;

    float _prevSpeed;
    float _prevAutoSpeed;
    float _prevBrakeVelocityLimit;
    float _prevFriction;

    private void Awake()
    {
        _carController = GetComponent<CarController>();
        InitAllNitros();
    }

    private void InitAllNitros()
    {
        Nitro[] nitros = FindObjectsByType<Nitro>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach (var nitro in nitros)
        {
            nitro.OnApplyNitro += ApplyNitro;
        }
    }

    private void ApplyNitro(Nitro nitro)
    {
        if (_nitroIE != null)
            StopCoroutine(_nitroIE);
        else
        {
            _prevSpeed = _carController.speed;
            _prevFriction = _carController.friction;
            _prevAutoSpeed = _carController.autoSpeed;
            _prevBrakeVelocityLimit = _carController.brakeToThisVelocityMagnitudeOnAutoMove;
        }

        _nitroIE = StartCoroutine(NitroIE());

        nitro.OnApplyNitro -= ApplyNitro;
    }

    IEnumerator NitroIE()
    {
        GameSoundAndHapticManager.Instance.PlaySoundAndHaptic(SoundType.nitro, false, _duration);

        _carController.speed = _nitroSpeedUpValue;
        _carController.friction = _frictionOnNitro;
        _carController.autoSpeed = _nitroSpeedUpValue;
        _carController.brakeToThisVelocityMagnitudeOnAutoMove = _brakeVelocityLimit;

        foreach (var fx in _nitroFXs)
        {
            fx.Play();
            //ParticleSystem.EmissionModule emit = fx.emission;
            //emit.enabled = true;
        }

        yield return new WaitForSeconds(_duration);

        foreach (var fx in _nitroFXs)
        {
            fx.Stop();
            //ParticleSystem.EmissionModule emit = fx.emission;
            //emit.enabled = false;
        }

        _carController.speed = _prevSpeed;
        _carController.friction = _prevFriction;
        _carController.autoSpeed = _prevAutoSpeed;
        _carController.brakeToThisVelocityMagnitudeOnAutoMove = _prevBrakeVelocityLimit;
    }
}
