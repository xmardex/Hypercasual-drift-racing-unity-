using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CarAIPhysicsSO", menuName = "Office_Driver/CarAIPhysicsSO", order = 0)]
public class CarAIPhysicsSO : ScriptableObject {
    [SerializeField] private CarMovementParameters _carMovementParameters_chase;
    [SerializeField] private CarMovementParameters _carMovementParameters_find;

    public CarMovementParameters CarMovementParametersForChase => _carMovementParameters_chase;
    public CarMovementParameters CarMovementParametersForFind => _carMovementParameters_find;
}

[Serializable]
public class CarMovementParameters{
    public float mass;
    public float turn;
    public float speed;
    public float friction;
    public float dragAmount;
}
