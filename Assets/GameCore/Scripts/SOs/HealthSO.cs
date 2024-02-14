using UnityEngine;
using System;

[CreateAssetMenu(fileName = "HealthSO", menuName = "Office_Driver/HealthSO", order = 0)]
public class HealthSO : ScriptableObject
{
    [SerializeField] private float _maxHP;
    [SerializeField] private float _minDamage;
    [SerializeField] private float _maxDamage;
    [SerializeField] private float _minDamageFactorValue;
    [SerializeField] private float _maxDamageFactorValue;

    public float MaxHP => _maxHP;
    public float MinDamage => _minDamage;
    public float MaxDamage => _maxDamage;
    public float MinDamageFactorValue => _minDamageFactorValue;
    public float MaxDamageFactorValue => _maxDamageFactorValue;
}