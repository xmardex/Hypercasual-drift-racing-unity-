using System;
using UnityEngine;

[Serializable]
public class PoliceCarContainer
{
    public CarAIParametersHolderSO carAIParametersHolder;
    public Transform spawnPoint;
    public CarAI carPrefab;
    public CarAIParametersSO carAIParametersSO;

    public CarAI SpawnCar()
    {
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.y += 2;
        return GameObject.Instantiate(carPrefab, spawnPosition, spawnPoint.rotation).GetComponent<CarAI>();
    }
}