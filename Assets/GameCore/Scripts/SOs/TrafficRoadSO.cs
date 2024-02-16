using UnityEngine;

[CreateAssetMenu(fileName = "TrafficRoadSO", menuName = "Office_Driver/TrafficRoadSO", order = 0)]
public class TrafficRoadSO : ScriptableObject 
{
    [SerializeField] private float _carsSpeed;
    [SerializeField] private float _carsRotateSpeed;

    public float ÑarsSpeed => _carsSpeed;
    public float ÑarsRotateSpeed => _carsRotateSpeed;
}