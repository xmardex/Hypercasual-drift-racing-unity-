using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CarsInitializator : MonoBehaviour
{
    [SerializeField] private bool _spawnPlayer;
    public CarController PrespawnedPlayer => _prespawnedPlayer;
    [SerializeField] private PlayerCarContainer _playerCarContainer;

    [Space(20) , Header("AI Cars:")]
    [Header("It is set relative to the length of the spline, so you need to set it to the appropriate one by testing.")]
    

    [Space(10)]
    [SerializeField] private float levelPlayerDetectionDistance;
    [SerializeField] private PoliceCarContainer[] _policeCarsContainers;

    private CarController _prespawnedPlayer;

    private List<CarController> _allCars;
    private List<CarAI> _allAICars;
    public List<CarAI> AllAICars => _allAICars;

    private SplineContainer _roadSpline;

    private float _playerResetDistance;
    private float _carAIResetDistance;
    public float _levelPlayerPointerOffset;

    private void Awake()
    {
        _prespawnedPlayer = GameObject.FindWithTag(Constants.PLAYER_CAR_TAG)?.GetComponent<CarController>();
        _roadSpline = GameObject.FindGameObjectWithTag(Constants.ROAD_SPLINE_CONTAINER_TAG).GetComponent<SplineContainer>();

        CalculateLevelSplineDistances();

        if (_prespawnedPlayer == null && !_spawnPlayer)
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
        playerCar.GetComponent<CarStuck>().SetResetDistance(_playerResetDistance);
        _allCars.Add(playerCar);

        foreach (PoliceCarContainer car in _policeCarsContainers)
        {
            if (!car.isActive)
                continue;
            CarAI policeCar = car.SpawnCar();
            policeCar.Initialize(playerCar, car.carAIParametersSO, levelPlayerDetectionDistance, _levelPlayerPointerOffset, car.carAIMovementParametersHolder);
            policeCar.GetComponent<CarReferences>().CarHealth.EnableHealthSystem(_useHP);
            policeCar.GetComponent<CarAIStuck>().SetResetDistance(_carAIResetDistance);
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

    private void CalculateLevelSplineDistances()
    {
        float roadSplineLength = _roadSpline.Spline.GetLength();

        _playerResetDistance = (Constants.ROAD_SPLINE_LENGTH_ETALON / roadSplineLength) * Constants.PLAYER_RESET_DISTANCE_ON_SPLINE_K;
        _carAIResetDistance = (Constants.ROAD_SPLINE_LENGTH_ETALON / roadSplineLength) * Constants.POLICE_RESET_DISTANCE_ON_SPLINE_K;
        _levelPlayerPointerOffset = (Constants.ROAD_SPLINE_LENGTH_ETALON / roadSplineLength) * Constants.LEVEL_PLAYER_POINTER_OFFSET;

        _playerResetDistance = -_playerResetDistance;
        _carAIResetDistance = -_carAIResetDistance;
    }

}
