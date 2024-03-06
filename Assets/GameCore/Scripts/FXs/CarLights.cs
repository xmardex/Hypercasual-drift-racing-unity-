using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLights : MonoBehaviour
{
    [SerializeField] private GameObject _light;

    public void EnableLights(bool enable)
    {
        _light.SetActive(enable);
    }
}
