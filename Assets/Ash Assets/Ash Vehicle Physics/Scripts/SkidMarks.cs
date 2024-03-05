using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidMarks : MonoBehaviour
{
    public Rigidbody wheel;
    private TrailRenderer skidMark;
    private ParticleSystem smoke;
    public CarController carController;
    private void Awake()
    {
        smoke = GetComponent<ParticleSystem>();
        skidMark = GetComponent<TrailRenderer>();
	    skidMark.emitting = false;
        skidMark.startWidth = carController.skidWidth;
    }
    
    
	private void OnEnable()
	{
		skidMark.enabled = true;
	}
	private void OnDisable()
	{
		skidMark.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        Vector3 velocity = wheel.transform.InverseTransformDirection(wheel.velocity);
        

        if (carController.grounded)
        {

            if (Mathf.Abs(velocity.x) > carController.SkidEnable || carController.IsBraking)
            {
                skidMark.emitting = true;
            }
            else
            {
                skidMark.emitting = false;
            }
        }
        else
        {
            skidMark.emitting = false;
        }

        // smoke
        if (skidMark.emitting == true)
        {
            smoke.Play();
        }
        else { smoke.Stop(); }

    }
}
