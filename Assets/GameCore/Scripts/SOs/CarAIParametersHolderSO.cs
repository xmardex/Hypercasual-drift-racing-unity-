using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CarAIParametersHolderSO", menuName = "Office_Driver/CarAIParametersHolderSO", order = 0)]
public class CarAIParametersHolderSO : ScriptableObject {
    [SerializeField] private CarMovementParameters _carMovementParameters_chase;
    [SerializeField] private CarMovementParameters _carMovementParameters_find;

    public CarMovementParameters CarMovementParametersForChase => _carMovementParameters_chase;
    public CarMovementParameters CarMovementParametersForFind => _carMovementParameters_find;
}

[Serializable]
public class CarMovementParameters
{
    [SerializeField] private float _mass;
    [SerializeField] private float _turn;
    [SerializeField] private float _speed;
    [SerializeField] private float _friction;
    [SerializeField] private float _dragAmount;
    [SerializeField] private float _distanceToPointer;

    public float Mass => _mass;
    public float Turn => _turn;
    public float Speed => _speed;
    public float Friction => _friction;
    public float DragAmount => _dragAmount;
    public float DistanceToPointer => _distanceToPointer;
}
