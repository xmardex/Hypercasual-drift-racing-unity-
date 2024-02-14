using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = GameObject.FindWithTag(Constants.OVERLAY_CAMERA_TAG).transform;
        if(_cameraTransform == null)
        {
            Debug.LogError("Camera no found", this);
        }
    }

    private void LateUpdate()
    {
        if(_cameraTransform != null)
            transform.LookAt(_cameraTransform);
    }
}
