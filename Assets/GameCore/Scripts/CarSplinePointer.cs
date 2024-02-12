using UnityEngine;
using UnityEngine.Splines;

public class CarSplinePointer : MonoBehaviour
{
    //distance to car
    [SerializeField] private float _maxDistance;

    //lerp speed to handle same distance with car by speed changing;
    [SerializeField] private float _pointerSpeedLerp;

    private Transform _carTransform;
    private SplineContainer _splineContainer;

    private float _splineLength;
    private float _distancePercentage = 0f;
    public float DistancePercentage => _distancePercentage;

    public void Initialize(Transform carTransform, SplineContainer roadSpline)
    {
        _splineContainer = roadSpline;
        _carTransform = carTransform;
        _splineLength = _splineContainer.Spline.GetLength();
    }

    public void UpdatePointerPosition(float carSpeed)
    {
        float distanceToTarget = Vector3.Distance(transform.position, _carTransform.position);

        if (distanceToTarget > _maxDistance)
        {
            float speedReductionFactor = Mathf.Clamp01((distanceToTarget - _maxDistance) / _maxDistance);
            carSpeed *= (1f - speedReductionFactor);
        }

        _distancePercentage += carSpeed * Time.deltaTime / _splineLength;
        _distancePercentage = Mathf.Clamp01(_distancePercentage);

        Vector3 currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);

        transform.position = currentPosition;
    }

    /// <summary>
    /// From 0 to 1 - new distance of pointer on spline
    /// </summary>
    /// <param name="newDistance"></param>
    public void ChangePointerOnSplineDistance(float newDistance)
    {
        _distancePercentage = newDistance;
    }
}
