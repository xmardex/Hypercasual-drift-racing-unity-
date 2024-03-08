using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    private bool _isLightsOn;
    public bool IsLightsOn => _isLightsOn;

    public void EnableLights(bool enable)
    {
        _isLightsOn = enable;
        _light.SetActive(enable);
    }
}
