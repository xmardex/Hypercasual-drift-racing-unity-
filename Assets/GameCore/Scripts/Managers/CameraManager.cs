using UnityEngine;
using Cinemachine;
using System;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Camera _overLayCamera;

    [Space(20),Header("Camera switchers:")]
    [SerializeField] private int activeCameraPriority = 20;
    [SerializeField] private int disabledCameraPriority = 10;
    [SerializeField] private CameraLevelPosition[] _cameraLevelPositions;

    private CarSplinePointer _playerCarSplinePointer;

    private int _currentPositionIndex;
    private CameraLevelPosition _currentCameraPosition;
    private CameraLevelPosition _nextCameraPosition;

    public void Initialize()
    {
        _currentPositionIndex = 0;
        _currentCameraPosition = _cameraLevelPositions[_currentPositionIndex];
        _nextCameraPosition = _cameraLevelPositions[_currentPositionIndex + 1];

        _playerCarSplinePointer = GameObject.FindGameObjectWithTag(Constants.PLAYER_SPLINE_POINTER).GetComponent<CarSplinePointer>();
    }

    public void LateUpdate()
    {
        if(_playerCarSplinePointer.DistancePercentage > _nextCameraPosition._pointerDistanceToActivate)
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
}

[Serializable]
public class CameraLevelPosition
{
    public float _pointerDistanceToActivate;
    public CinemachineVirtualCamera _virtualCamera;
}