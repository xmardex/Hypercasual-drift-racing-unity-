using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PlayerCarSO", menuName = "Office_Driver/PlayerCarSO", order = 0)]
public class PlayerCarSO : ScriptableObject
{
    [Header("Rotation")]
    [SerializeField] private float _straightSteerAngleThreshold = 4;
    [SerializeField] private float _steerDumpingSpeed = 30;

    [Header("Accelaration")]
    [SerializeField] private float _autoSpeed = 750;
    [SerializeField] private float _brakeToThisVelocityMagnitudeOnAutoMove = 35;

    [Header("Car Stats forces")]
    [SerializeField] private float _turn = 2500;
    [SerializeField] private float _speed = 1800;
    [SerializeField] private float _brake = 2000;
    [SerializeField] private float _turnAngle = 36;

    [Header("DRIFT SETUPS -> more drit - less value")]
    [SerializeField] private float _distanceToPointer = 12;
    [SerializeField] private float _friction = 1600;
    [SerializeField] private float _dragAmount = 4.5f;

    public float StraightSteerAngleThreshold => _straightSteerAngleThreshold;
    public float SteerDumpingSpeed => _steerDumpingSpeed;

    public float AutoSpeed => _autoSpeed;
    public float BrakeToThisVelocityMagnitudeOnAutoMove => _brakeToThisVelocityMagnitudeOnAutoMove;

    public float Speed => _speed;
    public float Brake => _brake;
    public float TurnAngle => _turnAngle;

    public float Turn => _turn;
    public float Friction => _friction;
    public float DragAmount => _dragAmount;

    public float DistanceToPointer => _distanceToPointer;

}