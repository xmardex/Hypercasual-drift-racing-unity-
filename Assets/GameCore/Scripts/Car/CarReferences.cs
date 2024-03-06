using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarReferences : MonoBehaviour
{
    [SerializeField] private CarHealth carHealth;
    public CarHealth CarHealth => carHealth;

    [SerializeField] private CarController carController;
    public CarController CarController => carController;

    [SerializeField] private CarStuck _carStuck;
    public CarStuck CarStuck => _carStuck;

    [SerializeField] private CarAIStuck _carAIStuck;
    public CarAIStuck CarAIStuck => _carAIStuck;

    [SerializeField] private CarDeath _carDeath;
    public CarDeath CarDeath => _carDeath;

    [SerializeField] private CollisionDetector collisionDetector;
    public CollisionDetector CollisionDetector => collisionDetector;

    [SerializeField] private CarLights _carLights;
    public CarLights CarLights => _carLights;
}
