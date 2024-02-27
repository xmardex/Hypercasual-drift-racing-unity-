using System;
using UnityEngine;

[Serializable]
public class PoliceCarContainer
{
    public bool isActive;
    public CarAIMovementParametersHolderSO carAIMovementParametersHolder;
    public Transform spawnPoint;
    public CarAI carPrefab;
    public CarAIParametersSO carAIParametersSO;
    public float resetSplineDistance;

    public CarAI SpawnCar()
    {
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.y += 2;
        return GameObject.Instantiate(carPrefab, spawnPosition, spawnPoint.rotation).GetComponent<CarAI>();
    }
}