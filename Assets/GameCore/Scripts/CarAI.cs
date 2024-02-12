using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(ÑarController))]
public class CarAI : MonoBehaviour
{
    private ÑarController _playerCar;
    private ÑarController _carController;
    private CarSplinePointer _carSplinePointer;

    private float _distanceForDetectPlayer;
    private float _playerPointerOffset;

    private float _lerpSpeedForChaseSpotSpeedLerp;
    private float _minChaseSpotLerpSpeed;
    private float _maxChaseSpotLerpSpeed;

    private float _minPlayerChaseSpotOffset;
    private float _maxPlayerChaseSpotOffset;

    private float _carSpeedAddedValue;
    private float _maxDistanceToAddSpeedValue;

    private bool _sleepUntilPlayerDetected;

    private bool _chaseTarget;
    private bool _findTarget;

    private Transform _splinePointerTarget;
    private Transform _thisAIchaseSpot;

    private float _currentChaseSpotLerpSpeed;
    private float _newSpeedValue;

    public void Initialize(ÑarController playerCar, CarAIParameters carAIParameters, float levelPlayerDetectionDistance, float levelPlayerPointerOffset)
    {
        _carController = GetComponent<ÑarController>();
        _carController.Initialize();

        _carSplinePointer = _carController.CarSplinePointer;

        _splinePointerTarget = _carSplinePointer.transform;
        _thisAIchaseSpot = _carController.ChaseSpot;

        _minPlayerChaseSpotOffset = carAIParameters.minChaseSpotOffset;
        _maxPlayerChaseSpotOffset = carAIParameters.maxChaseSpotOffset;
        _distanceForDetectPlayer = levelPlayerDetectionDistance;
        _playerPointerOffset = levelPlayerPointerOffset;
        _sleepUntilPlayerDetected = carAIParameters.sleepUntilPlayerDetect;
        _minChaseSpotLerpSpeed = carAIParameters.minChaseSpotLerpSpeed;
        _maxChaseSpotLerpSpeed = carAIParameters.maxChaseSpotLerpSpeed;
        _lerpSpeedForChaseSpotSpeedLerp = carAIParameters.lerpSpeedForChaseSpotSpeedLerp;
        _carSpeedAddedValue = carAIParameters.carSpeedAddedValue;
        _maxDistanceToAddSpeedValue = carAIParameters.maxDistanceToAddSpeedValue;

        _playerCar = playerCar;

        _carController.ChaseSpot.SetParent(_playerCar.ChaseSpot);

        if (_sleepUntilPlayerDetected)
        {
            _carController.ChangeTarget(null);
        }
        else
        {
            _carController.ChangeTarget(_splinePointerTarget);
        }
    }

    private void Update()
    {
        bool isPlayerReachable = IsPlayerReachable();
        float distanceToPlayer = GetDistanceToPlayer();
        bool isPlayerNear = distanceToPlayer < _distanceForDetectPlayer;

        if (!_sleepUntilPlayerDetected)
        {
            //looking for player
            if (isPlayerNear && isPlayerReachable)
            {
                //Player chasing
                if (!_chaseTarget)
                {
                    ChasePlayer();
                    _chaseTarget = true;
                    _findTarget = false;
                }
            }
            if(!isPlayerNear)
            {
                //Try follow spline until player detected
                if (!_findTarget)
                {
                    FindPlayer();
                    _findTarget = true;
                    _chaseTarget = false;
                }
            }
        }
        else
        {
            if(distanceToPlayer < _distanceForDetectPlayer && isPlayerReachable)
            {
                _sleepUntilPlayerDetected = false;
            }
        }

        if(isPlayerNear)
        {
            _currentChaseSpotLerpSpeed = Mathf.Lerp(_currentChaseSpotLerpSpeed, Random.Range(_minChaseSpotLerpSpeed, _maxChaseSpotLerpSpeed), Time.deltaTime * _lerpSpeedForChaseSpotSpeedLerp);
            float lerpValue = Mathf.PingPong(Time.time * _currentChaseSpotLerpSpeed, 1f);
            float offsetX = Mathf.Lerp(_minPlayerChaseSpotOffset, _maxPlayerChaseSpotOffset, lerpValue);

            Vector3 newPosition = new Vector3(offsetX, 2f, 0);
            _thisAIchaseSpot.localPosition = newPosition;
        }
    }

    private void FixedUpdate()
    {
        if (!_sleepUntilPlayerDetected && _chaseTarget)
        {
            _newSpeedValue = _playerCar.GetCurrentSpeedValue()  + (GetDistanceToPlayer() < _maxDistanceToAddSpeedValue ? 0 : _carSpeedAddedValue);
            if (GetDistanceToPlayer() > _maxDistanceToAddSpeedValue)
                _carController.ChangeSpeedValue(_newSpeedValue);
            else
                _carController.ChangeSpeedValue(_newSpeedValue/1.2f);
        }
    }

    private void ChasePlayer() 
    {
        _carController.UseDefalutSpeed(false);

        if (_playerCar.ChaseSpot != null)
            _carController.ChangeTarget(_thisAIchaseSpot);
        else
            Debug.Log("Player car don't have Chasing spot");
    }

    private void FindPlayer()
    {
        _carController.UseDefalutSpeed(true);

        float playerLastSeenAtSplineDistance = _playerCar.CarSplinePointer.DistancePercentage;
        _carSplinePointer.ChangePointerOnSplineDistance(playerLastSeenAtSplineDistance - _playerPointerOffset);
        _carController.ChangeTarget(_splinePointerTarget);
    }

    private bool IsPlayerReachable()
    {
        if (GetDistanceToPlayer() < _distanceForDetectPlayer)
        {
            Vector3 directionToPlayer = _playerCar.transform.position - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, _distanceForDetectPlayer))
            {
                return hit.collider.CompareTag(Constants.PLYAER_CAR_TAG);
            }
            return false;
        }
        return false;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(_playerCar.transform.position, transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, _distanceForDetectPlayer);
    }
}
