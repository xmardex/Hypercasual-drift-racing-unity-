using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CarStuck : MonoBehaviour
{
    [SerializeField] private float _delayBeforeStartChecking = 5;
    [SerializeField] private float _resetInDuration = 3;
    [SerializeField] private float _blinkDuration = 0.3f;
    [SerializeField] private int _blinkAmount = 3;
    [SerializeField] private float _splineDistanceFromPointerOffset;
    [SerializeField] private float _minCarVelocityToStuck;
    [SerializeField] private float _resetHeightOffset;
    [SerializeField] private float _policeCarsDetectDistance;
    [SerializeField] private int[] _ignoringOnBlinkLayers;
    [SerializeField] private Vector3 _limitPointerDistanceToReset;
    [SerializeField] private MeshRenderer[] _meshRenderers;
    [SerializeField] private CarReferences _carReferences;
    [SerializeField] private Rigidbody[] _wheels;

    private LevelManager _levelManager;
    private CarController _carController;

    private Coroutine _carCheckForRespawnIE;

    private bool _checkForStuck = false;
    private int _playerCarLayer;

    List<Vector3> initialWheelPositions;
    List<Quaternion> initialWheelRotations;

    private CarSplinePointer _carSplinePointer;
    private SplineContainer _splineContainer;

    public void Initialize(LevelManager levelManager)
    {
        _levelManager = levelManager;
        _carController = _carReferences.CarController;
        _playerCarLayer = _carController.gameObject.layer;
        _carSplinePointer = _carController.CarSplinePointer;
        _splineContainer = _carSplinePointer.SplineContainer;

        StartCoroutine(WaitBeforCheckStuckIE());
    }

    public void StopCheck()
    {
        StopAllCoroutines();
        _checkForStuck = false;
    }

    public void Update()
    {
        if (_checkForStuck)
        {
            if (_carController.CarVelocityMagnitude <= _minCarVelocityToStuck)
                _carCheckForRespawnIE = StartCoroutine(CarCheckForRespawnIE());
            else if (_carCheckForRespawnIE != null)
                StopCoroutine(_carCheckForRespawnIE);
        }
    }

    private IEnumerator WaitBeforCheckStuckIE()
    {
        _checkForStuck = false;
        yield return new WaitForSecondsRealtime(_delayBeforeStartChecking);
        _checkForStuck = true;
    }

    private IEnumerator CarCheckForRespawnIE()
    {
        float t = 0;
        while (_checkForStuck)
        {
            if(t >= _resetInDuration)
            {
                if (IsPoliceNear())
                {
                    _levelManager.CarStuckWithPolice();
                    yield break;
                }
                else if (IsOutOfTrack())
                {
                    _levelManager.CarOffRoad();
                    StartCoroutine(CarRespawnIE());
                    StartCoroutine(WaitBeforCheckStuckIE());
                    yield break;
                }
                else
                {
                    _levelManager.CarStuckSimple();
                    StartCoroutine(CarRespawnIE());
                    StartCoroutine(WaitBeforCheckStuckIE());
                    yield break;
                }
            }
            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator CarRespawnIE()
    {
        foreach(int layer in _ignoringOnBlinkLayers)
            Physics.IgnoreLayerCollision(_playerCarLayer, layer);

        initialWheelPositions ??= new List<Vector3>();
        if (initialWheelPositions.Count > 0) 
            initialWheelPositions.Clear();

        initialWheelRotations ??= new List<Quaternion>();
        if (initialWheelRotations.Count > 0) 
            initialWheelRotations.Clear();

        _carController.RB.isKinematic = true;
        foreach (Rigidbody wheel in _wheels)
        {
            initialWheelPositions.Add(wheel.transform.localPosition);
            initialWheelRotations.Add(wheel.transform.localRotation);
            wheel.isKinematic = true;
        }

        ResetCarOnSpline();

        for (int i = 0; i < _blinkAmount*2; i++)
        {
            if(i == 0)
            {
                yield return new WaitForSecondsRealtime(0.1f);
                _carController.RB.isKinematic = false;
                for (int w = 0; w < _wheels.Length; w++)
                {
                    _wheels[w].transform.localPosition = initialWheelPositions[w];
                    _wheels[w].transform.localRotation = initialWheelRotations[w];
                    _wheels[w].isKinematic = false;
                }
            }

            yield return new WaitForSecondsRealtime(_blinkDuration);

            foreach (MeshRenderer renderer in _meshRenderers)
                renderer.enabled = !renderer.enabled;
        }

        foreach (MeshRenderer renderer in _meshRenderers)
            renderer.enabled = true;

        foreach (int layer in _ignoringOnBlinkLayers)
            Physics.IgnoreLayerCollision(_playerCarLayer, layer, false);
    }

    private void ResetCarOnSpline()
    {
        float resetOnDistancePercentage = _carSplinePointer.DistancePercentage + _splineDistanceFromPointerOffset;

        Vector3 positionToReset = _splineContainer.EvaluatePosition(resetOnDistancePercentage);
        

        Vector3 forwardPosition = _splineContainer.EvaluatePosition(resetOnDistancePercentage + 0.001f);
        Vector3 direction = forwardPosition - positionToReset;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        positionToReset.y += _resetHeightOffset;

        _carController.transform.position = positionToReset;
        _carController.transform.rotation = targetRotation;
    }

    private bool IsPoliceNear()
    {
        Collider[] carsNear = Physics.OverlapSphere(_carController.transform.position, _policeCarsDetectDistance);
        if(carsNear.Length > 0)
        {
            foreach(Collider c in carsNear)
            {
                if (c.CompareTag(Constants.POLICE_CAR_TAG))
                    return true;
            }
        }
        return false;
    }

    private bool IsOutOfTrack()
    {
        float xDifference = Mathf.Abs(_carController.transform.position.x - _carSplinePointer.transform.position.x);
        float yDifference = Mathf.Abs(_carController.transform.position.x - _carSplinePointer.transform.position.x);
        float zDifference = Mathf.Abs(_carController.transform.position.x - _carSplinePointer.transform.position.x);

        return xDifference > _limitPointerDistanceToReset.x || yDifference > _limitPointerDistanceToReset.y || zDifference > _limitPointerDistanceToReset.z;

    }

    //private void OnDrawGizmos()
    //{
    //    Color color = Color.blue;
    //    color.a = 0.5f;
    //    Gizmos.color = color;
    //    Gizmos.DrawWireSphere(transform.position, _policeCarsDetectDistance);
    //}
}
