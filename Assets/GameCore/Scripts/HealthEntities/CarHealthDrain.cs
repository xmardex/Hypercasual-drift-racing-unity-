using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarHealth))]
public class CarHealthDrain : MonoBehaviour
{
    private ContinuousCollisionDetector _detector;
    private CarHealth _health;

    private void Awake()
    {
        _health = GetComponent<CarHealth>();
        _detector = GetComponent<ContinuousCollisionDetector>();
    }


}
