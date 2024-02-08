using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class CarSplineTarget : MonoBehaviour
{
    public SplineContainer spline;
    public float speed = 1f;
    public float distancePrecentage = 0f;

    [SerializeField] float splineLength;

    private void Start()
    {
        splineLength = spline.CalculateLength();

    }

    private void Update()
    {
        distancePrecentage += speed * Time.deltaTime / splineLength;

        Vector3 currentPosition = spline.EvaluatePosition(distancePrecentage);
        transform.position = currentPosition;

        if(distancePrecentage > 1f)
        {
            distancePrecentage = 0f;
        }

        Vector3 nextPosition = spline.EvaluatePosition(distancePrecentage + 0.05f);
        Vector3 direction = nextPosition - currentPosition;
        transform.rotation = Quaternion.LookRotation(direction,transform.up);
    }
}
