using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetWorldCanvasEventCamera : MonoBehaviour
{
    private void Awake()
    {
        Camera camera = GameObject.FindWithTag(Constants.OVERLAY_CAMERA_TAG).GetComponent<Camera>();
        if(TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = camera;
        }       
    }
}
