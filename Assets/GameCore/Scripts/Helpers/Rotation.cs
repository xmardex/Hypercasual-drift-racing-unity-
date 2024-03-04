using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotationSpeed = 1.0f;
    [SerializeField] private Vector3 direction = Vector3.up;

    void Update()
    {
        transform.Rotate(direction * rotationSpeed * Time.deltaTime, Space.World);
    }
}
