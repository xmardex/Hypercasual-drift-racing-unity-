using UnityEngine;
using Cinemachine;
public class CameraManager : MonoBehaviour
{
    [SerializeField] private int activeCameraPriority = 20;
    [SerializeField] private int disabledCameraPriority = 10;

    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    [SerializeField] private CinemachineVirtualCamera _prestartCamera;

    [SerializeField] private Camera _overlayCamera;

    private void Start()
    {
        SwitchToPlayerCamera();
    }

    public void SwitchToPlayerCamera()
    {
        _playerCamera.Priority = activeCameraPriority;
        _prestartCamera.Priority = disabledCameraPriority;
        _overlayCamera.enabled = true;
    }
}
