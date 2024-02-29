using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _overlayCamera;

    [Space(20), Header("Camera fx:")]
    [SerializeField] private float _minDamageToShake = 3;
    [SerializeField] private float _shakeIntensity = 5f;
    [SerializeField] private float _shakeDuration = 0.5f;

    [Space(20),Header("Camera switchers:")]
    [SerializeField] private int _activeCameraPriority = 20;
    [SerializeField] private int _disabledCameraPriority = 10;
    [SerializeField] private int _chasingCameraPriority = 30;
    [SerializeField] private CameraLevelPosition[] _cameraLevelPositions;

    [SerializeField] private float _cameraHeightLerpSpeed = 4;

    [SerializeField] private float _minCameraHeight = 0.3f;
    [SerializeField] private float _maxCameraHeight = 2f;

    [SerializeField] private float _minCarVelocityToChangeCameraHeight = 0;
    [SerializeField] private float _maxCarVelocityToChangeCameraHeight = 50;

    [SerializeField] private CinemachineVirtualCamera _chasingCamera;
    [SerializeField] private LayerMask _carsAILayerMask;

    [SerializeField] private float _chasingDistanceToActivateCamera = 30;

    [SerializeField] private Transform _allCamerasParent;
    private List<CinemachineCameraOffset> _camerasOffsets;

    private CarReferences _playerCarReferences;
    private CarController _playerCarController;
    private CarSplinePointer _playerCarSplinePointer;

    private int _currentPositionIndex;
    private CameraLevelPosition _currentCameraPosition;
    private CameraLevelPosition _nextCameraPosition;

    private bool _isChasingCameraActive;

    private Coroutine _shakeIE;

    public void Initialize()
    {
        _currentPositionIndex = 0;
        _currentCameraPosition = _cameraLevelPositions[_currentPositionIndex];
        _nextCameraPosition = _cameraLevelPositions[_currentPositionIndex + 1];
        _playerCarReferences = GameObject.FindGameObjectWithTag(Constants.PLAYER_CAR_TAG).GetComponent<CarReferences>();
        _playerCarController = _playerCarReferences.CarController;
        _playerCarSplinePointer = _playerCarReferences.CarController.CarSplinePointer;
        _playerCarReferences.CarHealth.OnDamage += ShakeCurrentCamera;
        UpdateFollowAndLook();
        InitAllCamerasOffsets();
    }

    private void UpdateFollowAndLook()
    {
        foreach(CameraLevelPosition cameraLevelPosition in _cameraLevelPositions)
        {
            cameraLevelPosition.virtualCamera.LookAt = _playerCarController.transform;
            cameraLevelPosition.virtualCamera.Follow = _playerCarController.transform;
        }
        _chasingCamera.LookAt = _playerCarController.transform;
        _chasingCamera.Follow = _playerCarController.transform;
    }

    private void InitAllCamerasOffsets()
    {
        _camerasOffsets ??= new List<CinemachineCameraOffset>();
        foreach(Transform t in _allCamerasParent) 
        {
            if(t.TryGetComponent(out CinemachineCameraOffset cco))
            {
                _camerasOffsets.Add(cco);
            }
        }
    }

    public void LateUpdate()
    {
        CheckForChase();
        UpdateCameraHeight();
        if (_playerCarSplinePointer.DistancePercentage > _nextCameraPosition.pointerDistanceToActivate)
        {
            SwitchCameraToNext();
        }

    }

    private void SwitchCameraToNext()
    {
        _currentCameraPosition.virtualCamera.Priority = _disabledCameraPriority;
        _nextCameraPosition.virtualCamera.Priority = _nextCameraPosition.overridePriority > 0 ? _nextCameraPosition.overridePriority : _activeCameraPriority;
        _currentCameraPosition = _nextCameraPosition;
        _currentPositionIndex++;
        if (_currentPositionIndex + 1 < _cameraLevelPositions.Length)
            _nextCameraPosition = _cameraLevelPositions[_currentPositionIndex+1];
    }

    public void EnableOverlays(bool enable)
    {
        _overlayCamera.enabled = enable;
    }

    private void UpdateCameraHeight()
    {
        float actualHeight = CalculateCameraHeight(_playerCarController.CarVelocityMagnitude);
        foreach(CinemachineCameraOffset cinemachineCameraOffset in _camerasOffsets)
        {
            cinemachineCameraOffset.m_Offset.z = Mathf.Lerp(cinemachineCameraOffset.m_Offset.z, -actualHeight, Time.deltaTime * _cameraHeightLerpSpeed);
        }
    }

    private float CalculateCameraHeight(float carVelocity)
    {
        float actualHeight = 0f;

        if (carVelocity < _minCarVelocityToChangeCameraHeight)
        {
            actualHeight = _minCameraHeight;
        }
        else if (carVelocity >= _minCarVelocityToChangeCameraHeight && carVelocity <= _maxCarVelocityToChangeCameraHeight)
        {
            float factorRange = _maxCarVelocityToChangeCameraHeight - _minCarVelocityToChangeCameraHeight;
            float heightRange = _maxCameraHeight;

            float factorRelativeToRange = (carVelocity - _minCarVelocityToChangeCameraHeight) / factorRange;
            actualHeight = factorRelativeToRange * heightRange;
        }
        else
        {
            actualHeight = _maxCameraHeight;
        }
        return actualHeight;
    }

    private void CheckForChase()
    {
        Collider[] colliders = Physics.OverlapSphere(_playerCarController.transform.position, _chasingDistanceToActivateCamera, _carsAILayerMask);

        bool isChasingAny = false;
        foreach (Collider col in colliders)
        {
            if (isChasingAny)
                break;
            isChasingAny = true; 
        }

        if (isChasingAny)
        {
            //_chasingCamera.LookAt = carAI.transform;
            _chasingCamera.Priority = _chasingCameraPriority;
            _isChasingCameraActive = true;
        }
        else
        {
            _chasingCamera.Priority = _disabledCameraPriority;
            _isChasingCameraActive = false;
        }
    }

    private void ShakeCurrentCamera(float damageValue)
    {
        //Debug.Log(damageValue);
        if (damageValue >= _minDamageToShake)
        {
            //TODO: May be use hitFactor for shakeIntensity
            CinemachineVirtualCamera currentCamera = (CinemachineVirtualCamera)CinemachineCore.Instance.GetActiveBrain(0).ActiveVirtualCamera;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            if (_shakeIE != null)
            {
                StopCoroutine(_shakeIE);
                Noise(cinemachineBasicMultiChannelPerlin, 0, 0);
            }
            _shakeIE = StartCoroutine(ProcessShakeIE(cinemachineBasicMultiChannelPerlin));
        }
    }
    private IEnumerator ProcessShakeIE(CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin)
    {
        Noise(cinemachineBasicMultiChannelPerlin, 1, _shakeIntensity);
        yield return new WaitForSeconds(_shakeDuration);
        Noise(cinemachineBasicMultiChannelPerlin,0, 0);
    }

    private void Noise(CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin, float amplitudeGain, float frequencyGain)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = amplitudeGain;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frequencyGain;
    }

}

[Serializable]
public class CameraLevelPosition
{
    public float pointerDistanceToActivate;
    public CinemachineVirtualCamera virtualCamera;
    public int overridePriority;
}