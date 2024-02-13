using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class CarsInitializator : MonoBehaviour
{
    public static CarsInitializator Instance;

    [SerializeField] private PlayerCarContainer _playerCar;
    [Space(20) , Header("AI Cars:")]
    [Range(0f, 1f)]
    public float levelPlayerPointerOffset;
    public float levelPlayerDetectionDistance;
    [SerializeField] private PoliceCarContainer[] _policeCars;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        InitializeCars();
    }

    public void InitializeCars()
    {
        CarController playerCar = _playerCar.SpawnCar();
        playerCar.Initialize();

        foreach(PoliceCarContainer car in _policeCars)
        {
            CarAI policeCar = car.SpawnCar();
            policeCar.Initialize(playerCar, car.carAIParametersSO, levelPlayerDetectionDistance, levelPlayerPointerOffset, car.carAIParametersHolder);
        }
    }
}
