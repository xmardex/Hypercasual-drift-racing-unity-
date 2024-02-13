using System;
using UnityEngine;

[Serializable]
public class PlayerCarContainer
{
    public Transform _spawnPoint;
    public СarController carPrefab;

    public СarController SpawnCar()
    {
        return GameObject.Instantiate(carPrefab,_spawnPoint.position,Quaternion.identity).GetComponent<СarController>();
    }
}
