using UnityEngine;

[CreateAssetMenu(fileName = "TrafficRoadSO", menuName = "Office_Driver/TrafficRoadSO", order = 0)]
public class TrafficRoadSO : ScriptableObject 
{
    [SerializeField] private float _carsSpeed;
    [SerializeField] private float _carsRotateSpeed;

    public float CarsSpeed => _carsSpeed;
    public float CarsRotateSpeed => _carsRotateSpeed;
}