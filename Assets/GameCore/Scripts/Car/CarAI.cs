using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CarController))]
public class CarAI : MonoBehaviour
{
    private CarController _playerCar;

    private CarController _carController;
    private CarSplinePointer _carSplinePointer;
    public CarSplinePointer CarSplinePointer => _carSplinePointer;

    private CarAIMovementParametersHolderSO _carAIParametersHolderSO;

    #region AiParameters
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
    #endregion
    
    private bool _chaseTarget;

    private bool _findTarget;

    private Transform _splinePointerTarget;
    private Transform _thisAIchaseSpot;

    private float _currentChaseSpotLerpSpeed;
    private float _newSpeedValue;

    private bool _active;

    private bool _chasingStartedOnce;

    public Action OnAIStartChasing;
    public Action OnAIDeactivated;

    public bool IsChasing => _chaseTarget;
    public bool IsFinding => _findTarget;
    public CarController PlayerCar => _playerCar;

    public void Initialize(CarController playerCar, CarAIParametersSO carAIParametersSO, float levelPlayerDetectionDistance, float levelPlayerPointerOffset, CarAIMovementParametersHolderSO carAIParametersHolderSO)
    {
        _carAIParametersHolderSO = carAIParametersHolderSO;

        _carController = GetComponent<CarController>();
        _carController.Initialize();

        _carSplinePointer = _carController.CarSplinePointer;

        _splinePointerTarget = _carSplinePointer.transform;
        _thisAIchaseSpot = _carController.ChaseSpot;

        _distanceForDetectPlayer = levelPlayerDetectionDistance;
        _playerPointerOffset = levelPlayerPointerOffset;

        SetupAIParameters(carAIParametersSO);

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
        _active = true;
    }

    private void SetupAIParameters(CarAIParametersSO carAIParametersSO)
    {
        _minPlayerChaseSpotOffset = carAIParametersSO.MinChaseSpotOffset;
        _maxPlayerChaseSpotOffset = carAIParametersSO.MaxChaseSpotOffset;

        _sleepUntilPlayerDetected = carAIParametersSO.SleepUntilPlayerDetect;
        _minChaseSpotLerpSpeed = carAIParametersSO.MinChaseSpotLerpSpeed;
        _maxChaseSpotLerpSpeed = carAIParametersSO.MaxChaseSpotLerpSpeed;
        _lerpSpeedForChaseSpotSpeedLerp = carAIParametersSO.LerpSpeedForChaseSpotSpeedLerp;
        _carSpeedAddedValue = carAIParametersSO.CarSpeedAddedValue;
        _maxDistanceToAddSpeedValue = carAIParametersSO.MaxDistanceToAddSpeedValue;
    }

    private void Update()
    {
        if (IsPlayerBehind() || !_active)
            return;

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

                    if (!_chasingStartedOnce)
                    {
                        OnAIStartChasing?.Invoke();
                        _chasingStartedOnce = true;
                    }
                    
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
        if (IsPlayerBehind() || !_active)
            return;

        if (!_sleepUntilPlayerDetected && _chaseTarget)
        {
            _newSpeedValue = _playerCar.GetCurrentSpeedValue() + (GetDistanceToPlayer() < _maxDistanceToAddSpeedValue ? 0 : _carSpeedAddedValue);
            _carController.ChangeSpeedValue(_newSpeedValue);
        }
    }

    private void ChasePlayer() 
    {
        _carController.ChangeMovementParameters(_carAIParametersHolderSO.CarMovementParametersForChase);
        _carController.UseDefalutSpeed(false);

        if (_playerCar.ChaseSpot != null)
            _carController.ChangeTarget(_thisAIchaseSpot);
        else
            Debug.Log("Player car don't have Chasing spot");
    }

    private void FindPlayer()
    {
        _carController.ChangeMovementParameters(_carAIParametersHolderSO.CarMovementParametersForFind);
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
                return hit.collider.CompareTag(Constants.PLAYER_CAR_TAG);
            }
            return false;
        }
        return false;
    }

    private bool IsPlayerBehind()
    {
        return _carController.CarSplinePointer.DistancePercentage > _playerCar.CarSplinePointer.DistancePercentage;
    }

    private float GetDistanceToPlayer()
    {
        return Vector3.Distance(_playerCar.transform.position, transform.position);
    }

    public void DeactivateAI()
    {
        _active = false;
        _chaseTarget = false;
        _findTarget = false;
        OnAIDeactivated?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, _distanceForDetectPlayer);
    }
}
