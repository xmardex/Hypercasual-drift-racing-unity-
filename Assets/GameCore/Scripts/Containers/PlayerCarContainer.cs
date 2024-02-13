using System;
using UnityEngine;

[Serializable]
public class PlayerCarContainer
{
    public Transform _spawnPoint;
    public CarController carPrefab;

    public CarController SpawnCar()
    {
        return GameObject.Instantiate(carPrefab,_spawnPoint.position,Quaternion.identity).GetComponent<CarController>();
    }
}
