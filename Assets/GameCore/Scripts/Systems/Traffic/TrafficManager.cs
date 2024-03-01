using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

public class TrafficManager : MonoBehaviour
{
    public TrafficCarController[] _trafficCars;

    [SerializeField] private TrafficRoadContainer[] _trafficRoadContainers;

    private List<TrafficCarController> _aciveTraficCars;
    private List<TrafficCarController> _crashedCard;
    private void Start()
    {
        InitializeTraffic();
    }

    public void InitializeTraffic()
    {
        _aciveTraficCars ??= new List<TrafficCarController>();
        _crashedCard ??= new List<TrafficCarController>();

        foreach (TrafficRoadContainer roadContainer in _trafficRoadContainers)
        {
            if(roadContainer.isActive)
                SpawnCarsOnRoad(roadContainer);
        }

        StartAllCars();
    }

    private void SpawnCarsOnRoad(TrafficRoadContainer trafficRoadContainer)
    {
        int count = trafficRoadContainer.carsCount;
        float _distanceBetweenCarsOnSpline = (1 - trafficRoadContainer.spawnOffsetFromEnd) / count;
        float _currentDistance = 0;

        for (int i = 0; i < count; i++)
        {
            TrafficCarController trafficCar = GetRandomCarPrefab(trafficRoadContainer);

            Vector3 carPosition = trafficRoadContainer.roadSpline.EvaluatePosition(_currentDistance);
            Vector3 carRotationEualr = trafficRoadContainer.roadSpline.EvaluateTangent(_currentDistance + trafficRoadContainer.lookAheadDistance);
            Quaternion carRotationQuaternion = Quaternion.LookRotation(carRotationEualr, Vector3.up);
            
            trafficCar = Instantiate(trafficCar, carPosition, carRotationQuaternion);
            trafficCar.gameObject.name = $"TrafficCar_{trafficRoadContainer.name}_{i}";
            trafficCar.Initialize(this, trafficRoadContainer, _currentDistance);

            _currentDistance += _distanceBetweenCarsOnSpline;

            _aciveTraficCars.Add(trafficCar);
        }
    }

    public void PlaceCarOnBegin(TrafficCarController trafficCarController, TrafficRoadContainer trafficRoadContainer)
    {
        Vector3 carPosition = trafficRoadContainer.roadSpline.EvaluatePosition(0);
        Vector3 carRotationEualr = trafficRoadContainer.roadSpline.EvaluateTangent(trafficRoadContainer.lookAheadDistance);

        trafficCarController.PlaceCarAt(carPosition, carRotationEualr);
    }

    public void RemoveCarFromList(TrafficCarController car)
    {
        _aciveTraficCars.Remove(car);
        _crashedCard.Add(car);
    }

    private void StartAllCars()
    {
        foreach(TrafficCarController trafficCar in _aciveTraficCars)
        {
            trafficCar.StartMoving();
        }
    }

    private TrafficCarController GetRandomCarPrefab(TrafficRoadContainer trafficRoadContainer)
    {
        return _trafficCars[Random.Range(0, _trafficCars.Length)];
    }
}

[Serializable]
public class TrafficRoadContainer
{
    public string name;
    public bool isActive;
    public int carsCount;
    [Header("Set this parameter low if spline are long")]
    [Range(0,0.5f)]
    public float lookAheadDistance;
    [Header("0.5f -> spawn until spline middle")]
    [Range(0,0.5f)]
    public float spawnOffsetFromEnd;
    public SplineContainer roadSpline;
    public TrafficRoadSO trafficRoadSO;
}