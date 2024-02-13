using UnityEngine;
using UnityEngine.Splines;

public class СarController : MonoBehaviour
{
    [Header("Spline inputs")]
    [Header("Pointer:")]
    [SerializeField] private CarSplinePointer _carSplinePointerPrefab;
    [SerializeField] private Transform _chaseSpot;
    private SplineContainer _roadSplineContainer;
    private CarSplinePointer _carSplinePointer;
    private Transform _carTarget;

    [Header("Rotation:")]
    [SerializeField] private float _straightSteerAngleThreshold;
    [SerializeField] private float _steerDumpingSpeed;

    [Header("Accelaration:")]
    [SerializeField] private float _autoSpeed;
    [SerializeField] private float _breakToThisVelocityMagnitudeOnAutoMove;

    [Header("Additional:")]
    [SerializeField] private float _maxTierRotationSpeed;

    [Header("AI:")]
    [SerializeField] private bool _isAI;
    [SerializeField] private bool _useDefaultSpeed;



    [Space(20)] [Header("Original")]
    public Transform groundCheck;
    public Transform fricAt;
    public Transform CentreOfMass;

    private Rigidbody rb;

    [Header("Car Stats")]
    public float speed = 200f;
    public float turn = 100f;
    public float brake = 150;
    public float friction = 70f;
    public float dragAmount = 4f;
    public float TurnAngle = 30f;

    public float maxRayLength = 0.8f;

    public bool grounded;

    [Header("Visuals")]
    public Transform[] TireMeshes;
    public Transform[] TurnTires;

    [Header("Curves")]
    public AnimationCurve frictionCurve;
    public AnimationCurve speedCurve;
    public AnimationCurve turnCurve;
    public AnimationCurve driftCurve;
    public AnimationCurve engineCurve;

    private float speedValue, autoSpeedValue, fricValue, turnValue, curveVelocity, brakeValue;
    [HideInInspector]
    public Vector3 carVelocity;
    [HideInInspector]
    public RaycastHit hit;

    [Header("Other Settings")]
    public AudioSource[] engineSounds;
    public bool airDrag;
    public float UpForce;
    public float SkidEnable = 20f;
    public float skidWidth = 0.12f;
    private float frictionAngle;

    private int _gasValue;

    public CarSplinePointer CarSplinePointer => _carSplinePointer;
    public Transform ChaseSpot => _chaseSpot;

    public void Initialize()
    {
        _roadSplineContainer = GameObject.FindGameObjectWithTag(Constants.ROAD_SPLINE_CONTAINER_TAG).GetComponent<SplineContainer>();
        _carSplinePointer = Instantiate(_carSplinePointerPrefab).GetComponent<CarSplinePointer>();
        _carSplinePointer.Initialize(transform,_roadSplineContainer);
        _carTarget = _carSplinePointer.transform;

        rb = GetComponent<Rigidbody>();
        grounded = false;
        engineSounds[1].mute = true;
        rb.centerOfMass = CentreOfMass.localPosition;
    }

    void FixedUpdate()
    {
        if (_carTarget != null)
        {
            carVelocity = transform.InverseTransformDirection(rb.velocity); //local velocity of car
            curveVelocity = Mathf.Abs(carVelocity.magnitude) / 100;

            float turnInput = turn * GetSteerPointerValue() * Time.fixedDeltaTime * 1000;

            float speedInput = speed * _gasValue * Time.fixedDeltaTime * 1000;
            float autoSpeedInput = _autoSpeed * Time.fixedDeltaTime * 1000;
            brakeValue = brake * Time.fixedDeltaTime * 1000;

            //helping veriables
            autoSpeedValue = autoSpeedInput * speedCurve.Evaluate(Mathf.Abs(carVelocity.z) / 100);
            if(!_isAI || _isAI && _useDefaultSpeed)
                speedValue = speedInput * speedCurve.Evaluate(Mathf.Abs(carVelocity.z) / 100);

            fricValue = friction * frictionCurve.Evaluate(carVelocity.magnitude / 100);
            turnValue = turnInput * turnCurve.Evaluate(carVelocity.magnitude / 100);

            //grounded check
            if (Physics.Raycast(groundCheck.position, -transform.up, out hit, maxRayLength))
            {
                AccelarationLogic();
                TurningLogic();
                FrictionLogic();
                //for drift behaviour
                rb.angularDrag = dragAmount * driftCurve.Evaluate(Mathf.Abs(carVelocity.x) / 70);

                //draws green ground checking ray ....ingnore
                Debug.DrawLine(groundCheck.position, hit.point, Color.green);
                grounded = true;

                rb.centerOfMass = Vector3.zero;
            }
            else
            {
                grounded = false;
                rb.drag = 0.1f;
                rb.centerOfMass = CentreOfMass.localPosition;
                if (!airDrag)
                {
                    rb.angularDrag = 0.1f;
                }
            }
        }
    }

    void Update()
    {
        if (_carTarget != null) 
        {
            _gasValue = _isAI || !_isAI && Input.GetKey(KeyCode.W) ? 1 : 0;
            _carSplinePointer.UpdatePointerPosition(carVelocity.magnitude);

            TireVisuals();
            AudioControl();
        }
    }

    public void AudioControl()
    {
        //audios
        if (grounded)
        {
            if (Mathf.Abs(carVelocity.x) > SkidEnable - 0.1f)
            {
                engineSounds[1].mute = false;
            }
            else { engineSounds[1].mute = true; }
        }
        else
        {
            engineSounds[1].mute = true;
        }

        engineSounds[1].pitch = 1f;

        engineSounds[0].pitch = 2 * engineCurve.Evaluate(curveVelocity);
        if (engineSounds.Length == 2)
        {
            return;
        }
        else 
        {
            engineSounds[2].pitch = 2 * engineCurve.Evaluate(curveVelocity); 
        }
    }

    public void TireVisuals()
    {
        //Tire mesh rotate
        foreach (Transform mesh in TireMeshes)
        {
	        mesh.transform.RotateAround(mesh.transform.position, mesh.transform.right, carVelocity.z/3);
            mesh.transform.localPosition = Vector3.zero;
        }

        foreach (Transform FM in TurnTires)
        {
            Vector3 currentRotation = FM.localRotation.eulerAngles;
            float targetAngle = TurnAngle * GetSteerPointerValue();

            Quaternion targetRotation = Quaternion.Euler(currentRotation.x, targetAngle, currentRotation.z);
            FM.localRotation = Quaternion.RotateTowards(FM.localRotation, targetRotation, _maxTierRotationSpeed * Time.deltaTime);
        }
    }

    public void AccelarationLogic()
    {
        if (_gasValue == 1)
        {
            rb.AddForceAtPosition(transform.forward * speedValue, groundCheck.position);
        }
        else
        {
            if (carVelocity.magnitude > _breakToThisVelocityMagnitudeOnAutoMove)
                rb.AddForceAtPosition(transform.forward * -brakeValue, groundCheck.position);
            else
                rb.AddForceAtPosition(transform.forward * autoSpeedValue, groundCheck.position);
        }
    }

    public void TurningLogic()
    {
        //turning
        if (carVelocity.z > 0.1f)
        {
            rb.AddTorque(transform.up * turnValue);
        }
        else if (GetSteerPointerValue() > 0.1f)
        {
            rb.AddTorque(transform.up * turnValue);
        }
        if (carVelocity.z < -0.1f && GetSteerPointerValue() < -0.1f)
        {
            rb.AddTorque(transform.up * -turnValue);
        }
    }

    public void FrictionLogic()
    {
        //Friction
        if (carVelocity.magnitude > 1)
        {
            frictionAngle = (-Vector3.Angle(transform.up, Vector3.up)/90f) + 1 ;
            rb.AddForceAtPosition(transform.right * fricValue * frictionAngle * 100 * -carVelocity.normalized.x, fricAt.position);
        }
    }

    #region CAR AI
    public void ChangeTarget(Transform newTarget)
    {
        _carTarget = newTarget;
    }
    public void ChangeSpeedValue(float newSpeedValue)
    {
        speedValue = newSpeedValue;
    }
    public float GetCurrentSpeedValue()
    {
        return _gasValue == 1 ? speedValue : autoSpeedValue;
    }
    public void UseDefalutSpeed(bool use)
    {
        _useDefaultSpeed = use;
    }
    public void ChangeMovementParameters(CarMovementParameters parameters)
    {
        turn = parameters.turn;
        speed = parameters.speed;
        friction = parameters.friction;
        rb.mass = parameters.mass;
    }
    #endregion


    private float GetSteerPointerValue()
    {
        if (_carTarget != null)
        {
            Vector3 targetDirection = _carTarget.position - transform.position;
            float angle = Vector3.SignedAngle(transform.forward, targetDirection, Vector3.up);
            angle = Mathf.Repeat(angle + 180f, 360f) - 180f;

            if (angle > _straightSteerAngleThreshold)
            {
                return Mathf.Lerp(0f, 1f, Mathf.InverseLerp(_straightSteerAngleThreshold, 180f, angle) * _steerDumpingSpeed);
            }
            else if (angle < -_straightSteerAngleThreshold)
            {
                return Mathf.Lerp(0f, -1f, Mathf.InverseLerp(-_straightSteerAngleThreshold, -180f, angle) * _steerDumpingSpeed);
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            Debug.LogError("car target not set");
            return 0f;
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawLine(groundCheck.position, groundCheck.position - maxRayLength * groundCheck.up);
    //        Gizmos.DrawWireCube(groundCheck.position - maxRayLength * (groundCheck.up.normalized), new Vector3(5, 0.02f, 10));
    //        Gizmos.color = Color.magenta;
    //        if (GetComponent<BoxCollider>())
    //        {
    //            Gizmos.DrawWireCube(transform.position, GetComponent<BoxCollider>().size);
    //        }
    //        if (GetComponent<CapsuleCollider>())
    //        {
    //            Gizmos.DrawWireCube(transform.position, GetComponent<CapsuleCollider>().bounds.size);
    //        }
    //        float wheelRadius = TurnTires[0].parent.GetComponent<SphereCollider>().radius;
    //        float wheelYPosition = TurnTires[0].parent.parent.localPosition.y + TurnTires[0].parent.localPosition.y;
    //        maxRayLength = (groundCheck.localPosition.y - wheelYPosition + (0.05f + wheelRadius))+ _groundCheckRayLengtAddition;
    //    }
    //}
}
