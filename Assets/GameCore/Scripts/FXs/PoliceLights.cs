using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceLights : MonoBehaviour
{
    
    [SerializeField] private GameObject _redLight;
    [SerializeField] private GameObject _blueLight;

    [SerializeField] private float _flashInterval = 0.5f;
    [SerializeField] private bool _enableOnAwake = false;


    private CarAI _carAI;
    private DamagableEntity _damageEntity;
    private bool lightsOn = false;

    private Coroutine _falshLightsIE;

    private void Awake()
    {
        _carAI = GetComponentInParent<CarAI>();
        _damageEntity = GetComponentInParent<DamagableEntity>();

        if (_carAI != null)
            _carAI.OnAIStartChasing += StartLight;

        if (_damageEntity != null)
            _damageEntity.OnDead += StopLight;

        _redLight.SetActive(false);
        _blueLight.SetActive(false);

        if (_enableOnAwake)
            StartLight();
    }

    private void OnDestroy()
    {
        if(_carAI != null)
            _carAI.OnAIStartChasing -= StartLight;
        if (_damageEntity != null)
            _damageEntity.OnDead -= StopLight;
    }

    public void StartLight()
    {
        _falshLightsIE = StartCoroutine(FlashLights());
    }

    public void StopLight()
    {
        if (_falshLightsIE != null)
        {
            StopCoroutine(_falshLightsIE);
            _redLight.SetActive(false);
            _blueLight.SetActive(false);
        }
    }

    IEnumerator FlashLights()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0, _flashInterval));
        while (true)
        {
            _redLight.SetActive(!lightsOn);
            _blueLight.SetActive(lightsOn);

            lightsOn = !lightsOn;

            yield return new WaitForSeconds(_flashInterval);
        }
    }
}
