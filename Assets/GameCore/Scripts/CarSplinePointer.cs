using UnityEngine;
using UnityEngine.Splines;

public class CarSplinePointer : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _pointerSpeedLerp;

    private float _splineLength;
    private float _distancePercentage = 0f;

    private void Start()
    {
        _splineLength = _splineContainer.Spline.GetLength();
    }

    public void UpdatePointerPosition(float carSpeed, float distanceToPointer, Transform carTransform)
    {
        float distanceToTarget = Vector3.Distance(transform.position, carTransform.position);

        if (distanceToTarget > _maxDistance)
        {
            float speedReductionFactor = Mathf.Clamp01((distanceToTarget - _maxDistance) / _maxDistance);
            carSpeed *= (1f - speedReductionFactor);
        }

        _distancePercentage += carSpeed * Time.deltaTime / _splineLength;
        _distancePercentage = Mathf.Clamp01(_distancePercentage);

        Vector3 currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);

        transform.position = currentPosition;

        if (_distancePercentage > 1f)
            _distancePercentage = 0;
    }
}
