using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Splines;


public class SplineCache : MonoBehaviour
{
    private SplineContainer _splineContainer;
    [Header("DONT CHANGE THOSE VALUES -> INDICATE PREBUILD CACHE")]
    [SerializeField] private bool _isCached;

    [SerializeField] private Vector3[] _cachedSplinePositions;
    private float splineCacheStep;

    public Vector3[] CachedSplinePositions => _cachedSplinePositions;

    private float _splineLength;

    public void CacheSpline()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineLength = _splineContainer.Spline.GetLength();
        splineCacheStep = Constants.SPLINE_CACHE_LEGTH_RESOLUTION / _splineLength;

        List<Vector3> cachedPositions = new List<Vector3>();

        for (float t = 0; t <= 1.0f; t += splineCacheStep)
        {
            Vector3 position = _splineContainer.EvaluatePosition(t);
            cachedPositions.Add(position);
        }
        Debug.Log("CACHED");
        _cachedSplinePositions = cachedPositions.ToArray();
        _isCached = true;
        Debug.Log("SPLINE CACHED SUCCESS " + _cachedSplinePositions.Length);
    }
}
