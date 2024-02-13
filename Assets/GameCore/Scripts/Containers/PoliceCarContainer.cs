using System;
using UnityEngine;

[Serializable]
public class PoliceCarContainer
{
    public Transform spawnPoint;
    public CarAI carPrefab;
    public CarAIParameters carAIParameters;

    public CarAI SpawnCar()
    {
        Vector3 spawnPosition = spawnPoint.position;
        spawnPosition.y += 2;
        return GameObject.Instantiate(carPrefab, spawnPosition, spawnPoint.rotation).GetComponent<CarAI>();
    }
}