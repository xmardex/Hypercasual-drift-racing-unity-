using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceLights : MonoBehaviour
{
    
    [SerializeField] private GameObject _redLight;
    [SerializeField] private GameObject _blueLight;

    [SerializeField] private float _flashInterval = 0.5f;

    private CarAI _carAI;

    private bool lightsOn = false;

    private void Awake()
    {
        _carAI = GetComponentInParent<CarAI>();
        _carAI.OnAIStartChasing += StartLight;
        _redLight.SetActive(false);
        _blueLight.SetActive(false);
    }

    private void OnDestroy()
    {
        _carAI.OnAIStartChasing -= StartLight;
    }

    public void StartLight()
    {
        StartCoroutine(FlashLights());
    }

    IEnumerator FlashLights()
    {
        while (true)
        {
            _redLight.SetActive(!lightsOn);
            _blueLight.SetActive(lightsOn);

            lightsOn = !lightsOn;

            yield return new WaitForSeconds(_flashInterval);
        }
    }
}
