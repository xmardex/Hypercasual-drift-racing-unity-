using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Transform _carSplineTarget;
    [SerializeField] private Rigidbody _carRb;
    [SerializeField] private float attractionForce = 10f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float targetDistance = 5f;
    [SerializeField] private float dampingFactor = 0.5f;

    private void FixedUpdate()
    {
        Vector3 directionToTarget = (_carSplineTarget.position - _carRb.position).normalized;
        Vector3 targetPosition = _carSplineTarget.position - directionToTarget * targetDistance;

        Vector3 forceDirection = (targetPosition - _carRb.position).normalized;
        _carRb.AddForce(forceDirection * attractionForce);

        float currentDistance = Vector3.Distance(_carRb.position, _carSplineTarget.position);
        float smoothDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref dampingFactor, 0.1f);

        Vector3 smoothTargetPosition = _carSplineTarget.position - directionToTarget * smoothDistance;

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget, Vector3.up);

        _carRb.MoveRotation(Quaternion.Slerp(_carRb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));

        _carRb.MovePosition(smoothTargetPosition);
    }
}