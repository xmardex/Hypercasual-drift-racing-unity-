using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CarAIParametersSO", menuName = "Office_Driver/CarAIParameters", order = 0)]
public class CarAIParametersSO : ScriptableObject 
{
    [SerializeField] private bool _sleepUntilPlayerDetect;

    [SerializeField] private float _minChaseSpotLerpSpeed;
    [SerializeField] private float _maxChaseSpotLerpSpeed;
    [SerializeField] private float _minChaseSpotOffset;
    [SerializeField] private float _maxChaseSpotOffset;
    [SerializeField] private float _lerpSpeedForChaseSpotSpeedLerp;

    [SerializeField] private float _carSpeedAddedValue;
    [SerializeField] private float _maxDistanceToAddSpeedValue;

    public bool SleepUntilPlayerDetect => _sleepUntilPlayerDetect;
    public float MinChaseSpotLerpSpeed => _minChaseSpotLerpSpeed;
    public float MaxChaseSpotLerpSpeed => _maxChaseSpotLerpSpeed;
    public float MinChaseSpotOffset => _minChaseSpotOffset;
    public float MaxChaseSpotOffset => _maxChaseSpotOffset;
    public float LerpSpeedForChaseSpotSpeedLerp => _lerpSpeedForChaseSpotSpeedLerp;
    public float CarSpeedAddedValue => _carSpeedAddedValue;
    public float MaxDistanceToAddSpeedValue => _maxDistanceToAddSpeedValue;
}

