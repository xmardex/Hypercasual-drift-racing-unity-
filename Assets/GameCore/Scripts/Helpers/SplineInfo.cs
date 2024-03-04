using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class SplineInfo : MonoBehaviour
{
    [SerializeField] private SplineContainer _splineContainer;

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (_splineContainer == null)
            return;

        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float labelX = 10f;
        float labelY = screenHeight - 30f;

        float labelWidth = 200f;
        float labelHeight = 20f;

        float splineLength = _splineContainer.Spline.GetLength();

        GUI.Label(new Rect(labelX, labelY, labelWidth, labelHeight), $"spline length: {splineLength}");

    }
#endif
}
