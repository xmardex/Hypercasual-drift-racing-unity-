using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineCache))]
public class SplineCacheEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SplineCache splineCache = (SplineCache)target;

        if (GUILayout.Button("Cache spline"))
        {
            splineCache.CacheSpline();
        }
    }
}