using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogHeighAdjust : MonoBehaviour
{
    private Transform _cameraT;
    private float _defaultHeightOffset;
    private void Awake()
    {
        _cameraT = GameObject.FindWithTag("MainCamera").transform;
        _defaultHeightOffset = _cameraT.position.y - transform.position.y;

    }

    private void LateUpdate()
    {
        Vector3 newPos = _cameraT.position;
        newPos.y -= _defaultHeightOffset;
        transform.position = newPos;
    }
}
