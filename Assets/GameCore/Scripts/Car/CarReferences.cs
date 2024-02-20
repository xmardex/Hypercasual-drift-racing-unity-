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
}
