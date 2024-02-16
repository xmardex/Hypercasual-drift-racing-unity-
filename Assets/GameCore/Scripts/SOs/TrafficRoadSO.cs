using UnityEngine;

[CreateAssetMenu(fileName = "TrafficRoadSO", menuName = "Office_Driver/TrafficRoadSO", order = 0)]
public class TrafficRoadSO : ScriptableObject 
{
    [SerializeField] private float _carsSpeed;
    [SerializeField] private float _carsRotateSpeed;

    public float �arsSpeed => _carsSpeed;
    public float �arsRotateSpeed => _carsRotateSpeed;
}