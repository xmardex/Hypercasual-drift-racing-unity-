using System;
using System.Collections.Generic;
using UnityEngine;

public class CarsInitializator : MonoBehaviour
{
    [SerializeField] private bool _spawnPlayer;
    public CarController PrespawnedPlayer => _prespawnedPlayer;
    [SerializeField] private PlayerCarContainer _playerCarContainer;

    [Space(20) , Header("AI Cars:")]
    [Header("It is set relative to the length of the spline, so you need to set it to the appropriate one by testing.")]
    [Range(0f, 1f)] public float levelPlayerPointerOffset;

    [Space(10)]
    [SerializeField] private float levelPlayerDetectionDistance;
    [SerializeField] private PoliceCarContainer[] _policeCarsContainers;

    private CarController _prespawnedPlayer;

    private List<CarController> _allCars;
    private List<CarAI> _allAICars;
    public List<CarAI> AllAICars => _allAICars;

    private void Awake()
    {
        _prespawnedPlayer = GameObject.FindWithTag(Constants.PLAYER_CAR_TAG)?.GetComponent<CarController>();
        if (_prespawnedPlayer == null)
        {
            Debug.LogError("Use prespawned player or spawn one");
            return;
        }
        _allCars ??= new List<CarController>();
        _allAICars ??= new List<CarAI>();
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
            policeCar.GetComponent<CarAIStuck>().SetResetDistance(car.resetSplineDistance);
            _allAICars.Add(policeCar);
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
