using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;


[RequireComponent(typeof(Rigidbody), typeof(CollisionDetector))]
public class TrafficCarController : MonoBehaviour
{
    private SplineContainer _road;
    private Rigidbody _rb;
    private CollisionDetector _collisionDetector;
    private TrafficManager _trafficManager;
    private TrafficRoadContainer _trafficRoadContainer;

    private float _lookAheadDistance;
    private float _carSpeed;
    private float _carRotateSpeed;

    private float _splineLength;
    private float _distancePercentage;

    private bool _isHitted;
    private bool _startMoving;

    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    public void Initialize(TrafficManager trafficManager, TrafficRoadContainer trafficRoadContainer, float startDistancePercentage)
    {
        _rb = GetComponent<Rigidbody>();
        _collisionDetector = GetComponent<CollisionDetector>();
        _trafficManager = trafficManager;
        _trafficRoadContainer = trafficRoadContainer;

        _road = trafficRoadContainer.roadSpline;
        _carSpeed = trafficRoadContainer.trafficRoadSO.ÑarsSpeed;
        _carRotateSpeed = trafficRoadContainer.trafficRoadSO.ÑarsRotateSpeed;
        _lookAheadDistance = trafficRoadContainer.lookAheadDistance;

        _splineLength = _road.CalculateLength();
        _collisionDetector.OnCollideWithSomething += DetectedHit;
        _distancePercentage = startDistancePercentage;

        _startMoving = false;
    }

    void FixedUpdate()
    {
        if (!_isHitted && _startMoving)
        {
            _distancePercentage += _carSpeed * Time.deltaTime / _splineLength;

            Vector3 currentPosition = _road.EvaluatePosition(_distancePercentage);
            Vector3 forwardPosition = _road.EvaluatePosition(_distancePercentage + _lookAheadDistance);
            Vector3 direction = forwardPosition - currentPosition;

            _targetPosition = currentPosition;
            _targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

            MoveAndRotate();

            if(_distancePercentage >= 0.99f)
            {
                _trafficManager.PlaceCarOnBegin(this, _trafficRoadContainer);
                _distancePercentage = 0;
            }
        }
    }

    void MoveAndRotate()
    {
        Vector3 velocity = (_targetPosition - _rb.position).normalized * _carSpeed;
        _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);

        _rb.rotation = Quaternion.Lerp(_rb.rotation, _targetRotation, Time.deltaTime * _carRotateSpeed);
    }
    private void DetectedHit(Collider by, float hitFactor)
    {
        _isHitted = true;
    }

    public void PlaceCarAt(Vector3 position, Vector3 rotation)
    {
        Vector3 newPosition = new Vector3(position.x, position.y+0.2f, position.z);
        transform.position = newPosition;
        transform.eulerAngles = rotation;
    }

    public void StartMoving() => _startMoving = true;

}
