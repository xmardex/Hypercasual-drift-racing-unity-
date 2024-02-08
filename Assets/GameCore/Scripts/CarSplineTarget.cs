using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class CarSplineTarget : MonoBehaviour
{
    [SerializeField] private carController _carController;
    [SerializeField] private SplineContainer _splineContainer;

    [SerializeField] private Transform _nearestMarker;

    [SerializeField] private float _splineTOffset;
    [SerializeField] private float _distanceOffset;

    [SerializeField] private int _resolution,_iteratons;
    float _splineLength;
    private float _splineMarkerOffset;

    private void Start()
    {
        _splineLength = _splineContainer.CalculateLength();
        _carController = GameObject.FindGameObjectWithTag("car").GetComponent<carController>();
    }
    private void Update()
    {
        if (_carController == null || _splineContainer == null || _nearestMarker == null)
        {
            Debug.LogWarning("Missing references. Make sure to assign all required objects.");
            return;
        }

        // Get the nearest point on the spline to the car's position
        float3 nearestPointOnSpline;
        float tNearest;
        float distanceToSpline = SplineUtility.GetNearestPoint(
            _splineContainer.Spline,
            new Ray(_carController.transform.position, Vector3.down),
            out nearestPointOnSpline,
            out tNearest,
            _resolution,
            _iteratons
        );

        // Calculate the new position of _nearestMarker based on the provided offset
        _nearestMarker.position = SplineUtility.GetPointAtLinearDistance(_splineContainer.Spline, tNearest + _splineTOffset, _distanceOffset, out float resultPointT);
    }
}

