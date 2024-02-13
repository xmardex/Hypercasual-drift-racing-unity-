using System;
using UnityEngine;

[Serializable]
public class PlayerCarContainer
{
    public Transform _spawnPoint;
    public CarController carPrefab;

    public CarController SpawnCar()
    {
        Vector3 spawnPosition = _spawnPoint.position;
        spawnPosition.y += 2f;

        return GameObject.Instantiate(carPrefab, spawnPosition, Quaternion.identity).GetComponent<CarController>();
    }
}
