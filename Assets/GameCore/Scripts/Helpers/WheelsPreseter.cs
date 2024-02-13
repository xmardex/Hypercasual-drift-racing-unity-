using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelsPreseter : MonoBehaviour
{
    [SerializeField] private CarWheelsJointSO _carWheelsJointsSO;
    [SerializeField] private ConfigurableJoint[] _configurableJoint;

    private void Awake() 
    {
        foreach(ConfigurableJoint joint in _configurableJoint)
        {
            var _linearLimitSpring = joint.linearLimitSpring;
            _linearLimitSpring.damper = _carWheelsJointsSO.LinierLimitSpring_damper;
            _linearLimitSpring.spring = _carWheelsJointsSO.LinierLimitSpring_spring;
            joint.linearLimitSpring = _linearLimitSpring;

            var linearLimit = joint.linearLimit;
            linearLimit.limit = _carWheelsJointsSO.LinearLimit_limit;
            linearLimit.contactDistance = _carWheelsJointsSO.LinearLimit_contactDistance;
            joint.linearLimit = linearLimit;

            var yDrive = joint.yDrive;
            yDrive.positionSpring = _carWheelsJointsSO.YDrive_positionSpring;
            yDrive.positionDamper = _carWheelsJointsSO.YDrive_positionDamper;
            joint.yDrive = yDrive;

            var angularYZLimitSpring = joint.angularYZLimitSpring;
            angularYZLimitSpring.spring = _carWheelsJointsSO.AngularYZLimitSpring_spring;
            angularYZLimitSpring.damper = _carWheelsJointsSO.AngularYZLimitSpring_damper;
            joint.angularYZLimitSpring = angularYZLimitSpring;

            var angularXLimitSpring = joint.angularXLimitSpring;
            angularXLimitSpring.spring = _carWheelsJointsSO.AngularXLimitSpring_spring;
            angularXLimitSpring.damper = _carWheelsJointsSO.AngularXLimitSpring_damper;
            joint.angularXLimitSpring = angularXLimitSpring;

            var angularZLimit = joint.angularZLimit;
            angularZLimit.limit = _carWheelsJointsSO.AngularZLimit_limit;
            joint.angularZLimit = angularZLimit;

        }
    }
}
