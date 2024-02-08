using UnityEngine;
using UnityEngine.Splines;

public class CarSplinePointer : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

    [SerializeField] private float _distancePercentage = 0f;
    private float _splineLength;
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _pointerSpeedLerp;

    private void Start()
    {
        _splineLength = _splineContainer.Spline.GetLength();
    }

    public void UpdatePointerPosition(float carSpeed, float distanceToPointer, Transform carTransform)
    {
        float distanceToTarget = Vector3.Distance(transform.position, carTransform.position);
        Debug.Log($"Distance: {distanceToTarget}");

        if (distanceToTarget > _maxDistance)
        {

            float speedReductionFactor = Mathf.Clamp01((distanceToTarget - _maxDistance) / _maxDistance);


            carSpeed *= (1f - speedReductionFactor);
        }
        Debug.Log($"speed: {carSpeed}");

        _distancePercentage += carSpeed * Time.deltaTime / _splineLength;
        _distancePercentage = Mathf.Clamp01(_distancePercentage);


        Vector3 currentPosition = _splineContainer.EvaluatePosition(_distancePercentage);


        transform.position = currentPosition;


        if (_distancePercentage > 1f)
            _distancePercentage = 0;
    }
}
