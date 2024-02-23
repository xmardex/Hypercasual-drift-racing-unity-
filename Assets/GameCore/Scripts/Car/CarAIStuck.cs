using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAIStuck : MonoBehaviour
{
    [SerializeField] private float _resetSplineDistanceOffsetFromPlayer;
    [SerializeField] private float _maxDistanceToPlayer;
    [SerializeField] private float _resetHeightOffset;
    [SerializeField] private float _invinsibleDuration;
    [SerializeField] private float _resetInDuration;
    [SerializeField] private float _delayBeforeStartChecking;
    [SerializeField] private int[] _ignoringOnResetLayers;
    [SerializeField] private Rigidbody[] _wheels;

    private CarAI _carAI;
    private Rigidbody _rb;
    private CarController _carController;
    private int _carLayer;

    private bool _stuckProcessing;

    private Vector3 _initialCarVelocity;

    List<Vector3> initialWheelPositions;
    List<Quaternion> initialWheelRotations;

    private Coroutine _carCheckForRespawnIE;

    private void Awake()
    {
        _carAI = GetComponent<CarAI>();
        _rb = GetComponent<Rigidbody>();
        _carController = GetComponent<CarController>();
        _carLayer = gameObject.layer;

        if (_rb == null || _carAI == null || _carController == null)
        {
            Debug.LogError("Wrong entity for this stuck script", this);
            return;
        }
    }
    public void FixedUpdate()
    {
        if (_carAI.IsFinding)
        {
            if (!_stuckProcessing && DistanceToPlayer() > _maxDistanceToPlayer)
            {
                _carCheckForRespawnIE = StartCoroutine(CarCheckForRespawnIE());
            }
            if (_stuckProcessing && DistanceToPlayer() <= _maxDistanceToPlayer)
            {
                _stuckProcessing = false;
                StopAllCoroutines();
            }
        }
        else {
            if (_stuckProcessing)
            {
                StopAllCoroutines();
                _stuckProcessing = false;
            }
        }
    }

    private IEnumerator CarCheckForRespawnIE()
    {
        _stuckProcessing = true;

        float t = 0;
        while (t < _resetInDuration)
        {
            t += Time.deltaTime;
            yield return null;
        }

         StartCoroutine(CarRespawnIE());
        
        _stuckProcessing = false;
    }

    private IEnumerator CarRespawnIE()
    {
        foreach (int layer in _ignoringOnResetLayers)
            Physics.IgnoreLayerCollision(_carLayer, layer);

        initialWheelPositions ??= new List<Vector3>();
        if (initialWheelPositions.Count > 0)
            initialWheelPositions.Clear();

        initialWheelRotations ??= new List<Quaternion>();
        if (initialWheelRotations.Count > 0)
            initialWheelRotations.Clear();

        //_initialCarVelocity = _carController.carVelocity;
        //_carController.RB.isKinematic = true;
        foreach (Rigidbody wheel in _wheels)
        {
            initialWheelPositions.Add(wheel.transform.localPosition);
            initialWheelRotations.Add(wheel.transform.localRotation);
            //wheel.isKinematic = true;
        }

        ResetCarOnSpline();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        //_carController.RB.isKinematic = false;
        //_carController.carVelocity = transform.InverseTransformDirection(_initialCarVelocity);
        for (int w = 0; w < _wheels.Length; w++)
        {
            _wheels[w].transform.localPosition = initialWheelPositions[w];
            _wheels[w].transform.localRotation = initialWheelRotations[w];
            //_wheels[w].isKinematic = false;
        }
        yield return new WaitForSecondsRealtime(_invinsibleDuration);
        foreach (int layer in _ignoringOnResetLayers)
            Physics.IgnoreLayerCollision(_carLayer, layer, false);
    }

    private void ResetCarOnSpline()
    {
        float resetOnDistancePercentage = _carAI.PlayerCar.CarSplinePointer.DistancePercentage + _resetSplineDistanceOffsetFromPlayer;

        Vector3 positionToReset = _carAI.CarSplinePointer.SplineContainer.EvaluatePosition(resetOnDistancePercentage);


        Vector3 forwardPosition = _carAI.CarSplinePointer.SplineContainer.EvaluatePosition(resetOnDistancePercentage + 0.005f);
        Vector3 direction = forwardPosition - positionToReset;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);

        positionToReset.y += _resetHeightOffset;

        _carAI.CarSplinePointer.ChangePointerOnSplineDistance(resetOnDistancePercentage + 0.005f);

        _carController.transform.position = positionToReset;
        _carController.transform.rotation = targetRotation;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(_carAI.transform.position, _carAI.PlayerCar.transform.position);
    }
}
