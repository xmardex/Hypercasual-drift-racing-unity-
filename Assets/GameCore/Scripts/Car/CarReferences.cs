using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarReferences : MonoBehaviour
{
    [SerializeField] private CarHealth carHealth;
    public CarHealth CarHealth => carHealth;
}
