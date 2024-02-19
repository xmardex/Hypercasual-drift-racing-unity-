using UnityEngine;
using Cinemachine;
using System;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _overLayCamera;

    [Space(20),Header("Camera switchers:")]
    [SerializeField] private int activeCameraPriority = 20;
    [SerializeField] private int disabledCameraPriority = 10;
    [SerializeField] private CameraLevelPosition[] _cameraLevelPositions;

    [SerializeField] private float _cameraHeightLerpSpeed = 4;

    [SerializeField] private float _minCameraHeight = 0.3f;
    [SerializeField] private float _maxCameraHeight = 2f;

    [SerializeField] private float _minCarVelocityToChangeCameraHeight = 0;
    [SerializeField] private float _maxCarVelocityToChangeCameraHeight = 50;

    [SerializeField] private Transform _allCamerasParent;
    private List<CinemachineCameraOffset> _camerasOffsets;

    private CarController _playerCarController;
    private CarSplinePointer _playerCarSplinePointer;

    private int _currentPositionIndex;
    private CameraLevelPosition _currentCameraPosition;
    private CameraLevelPosition _nextCameraPosition;

    public void Initialize()
    {
        _currentPositionIndex = 0;
        _currentCameraPosition = _cameraLevelPositions[_currentPositionIndex];
        _nextCameraPosition = _cameraLevelPositions[_currentPositionIndex + 1];
        _playerCarController = GameObject.FindGameObjectWithTag(Constants.PLYAER_CAR_TAG).GetComponent<CarController>();
        _playerCarSplinePointer = _playerCarController.CarSplinePointer;
        InitAllCamerasOffsets();
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
        UpdateCameraHeight();
        if (_playerCarSplinePointer.DistancePercentage > _nextCameraPosition._pointerDistanceToActivate)
        {
            SwitchCameraToNext();
        }
    }

    private void SwitchCameraToNext()
    {
        _currentCameraPosition._virtualCamera.Priority = disabledCameraPriority;
        _nextCameraPosition._virtualCamera.Priority = activeCameraPriority;
        _currentCameraPosition = _nextCameraPosition;

        _currentPositionIndex++;
        if (_currentPositionIndex + 1 < _cameraLevelPositions.Length)
            _nextCameraPosition = _cameraLevelPositions[_currentPositionIndex+1];
    }

    public void EnableOverlays(bool enable)
    {
        _overLayCamera.enabled = enable;
    }

    private void UpdateCameraHeight()
    {
        float actualHeight = CalculateCameraHeight(_playerCarController.CarVelocityMagnitude);
        foreach(CinemachineCameraOffset cinemachineCameraOffset in _camerasOffsets)
        {
            cinemachineCameraOffset.m_Offset.z = Mathf.Lerp(cinemachineCameraOffset.m_Offset.z, -actualHeight, Time.deltaTime * _cameraHeightLerpSpeed);
        }
    }

    public float CalculateCameraHeight(float carVelocity)
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
}

[Serializable]
public class CameraLevelPosition
{
    public float _pointerDistanceToActivate;
    public CinemachineVirtualCamera _virtualCamera;
}