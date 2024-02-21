using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Splines;

public class CarsInitializator : MonoBehaviour
{
    [SerializeField] private bool _spawnPlayer;
    [SerializeField] private CarController _prespawnedPlayer;
    [SerializeField] private PlayerCarContainer _playerCarContainer;

    [Space(20) , Header("AI Cars:")]
    [Header("It is set relative to the length of the spline, so you need to set it to the appropriate one by testing.")]
    [Range(0f, 1f)] public float levelPlayerPointerOffset;

    [Space(10)]
    [SerializeField] private float levelPlayerDetectionDistance;
    [SerializeField] private PoliceCarContainer[] _policeCarsContainers;

    private List<CarController> _allCars;

    private void Awake()
    {
        _allCars ??= new List<CarController>();
    }

    public void InitializeCars(bool _useHP, bool canMoveOnStart = false)
    {
        CarController playerCar = _spawnPlayer ? _playerCarContainer.SpawnCar() : _prespawnedPlayer;
        playerCar.Initialize();
        playerCar.GetComponent<CarReferences>().CarHealth.EnableHealthSystem(_useHP);

        _allCars.Add(playerCar);

        foreach (PoliceCarContainer car in _policeCarsContainers)
        {
            if (!car.isActive)
                continue;
            CarAI policeCar = car.SpawnCar();
            policeCar.Initialize(playerCar, car.carAIParametersSO, levelPlayerDetectionDistance, levelPlayerPointerOffset, car.carAIMovementParametersHolder);
            policeCar.GetComponent<CarReferences>().CarHealth.EnableHealthSystem(_useHP);

            _allCars.Add(policeCar.GetComponent<CarController>());
        }

        EnableMovementOnCars(canMoveOnStart);
    }

    public void EnableMovementOnCars(bool enable)
    {
        foreach(CarController car in _allCars)
        {
            car.SetCanMove(enable);
        }
    }

}
