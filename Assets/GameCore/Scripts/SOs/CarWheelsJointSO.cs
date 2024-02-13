using UnityEngine;

[CreateAssetMenu(fileName = "CarWheelsJointSO", menuName = "Office_Driver/CarWheelsJointSO", order = 0)]
public class CarWheelsJointSO : ScriptableObject {
    [SerializeField] private float _linearLimitSpring_spring;
    [SerializeField] private float _linearLimitSpring_damper;
    
    [SerializeField] private float _linearLimit_limit;
    [SerializeField] private float _linearLimit_contactDistance;

    [SerializeField] private float _yDrive_positionSpring;
    [SerializeField] private float _yDrive_positionDamper;

    [SerializeField] private float _angularYZLimitSpring_spring;
    [SerializeField] private float _angularYZLimitSpring_damper;

    [SerializeField] private float _angularXLimitSpring_spring;
    [SerializeField] private float _angularXLimitSpring_damper;

    [Range(0,177)]
    [SerializeField] private float _angularZLimit_limit;

    public float LinierLimitSpring_spring => _linearLimitSpring_spring;

    public float LinierLimitSpring_damper=> _linearLimitSpring_damper;
    public float LinearLimit_limit=> _linearLimit_limit;
    public float LinearLimit_contactDistance=> _linearLimit_contactDistance;
    public float YDrive_positionSpring=> _yDrive_positionSpring;
    public float YDrive_positionDamper=> _yDrive_positionDamper;

    public float AngularYZLimitSpring_spring => _angularYZLimitSpring_spring;
    public float AngularYZLimitSpring_damper=> _angularYZLimitSpring_damper;
    public float AngularZLimit_limit=> _angularZLimit_limit;

    public float AngularXLimitSpring_spring => _angularXLimitSpring_spring;
    public float AngularXLimitSpring_damper => _angularXLimitSpring_damper;
}