using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWheelBasedMovement : MonoBehaviour
{

	/*
	*Group of variables that build out the base functionality of the movement class for all cars.
	*/
	private float horizontalInput;
	private float verticalInput;
	private float steeringAngle;

	public WheelCollider frontDriverW, frontPassengerW;
	public WheelCollider rearDriverW, rearPassengerW;
	public Transform frontDriverT, frontPassengerT;
	public Transform rearDriverT, rearPassengerT;
	public float maxSteerAngle = 30f;
	public float motorForce = 50f;

	private void FixedUpdate()
	{
		GetInput();
		Steer();
		Accelerate();
		UpdateWheelPose();
	}

	public void GetInput()
	{
		horizontalInput = Input.GetAxis("Horizontal");
		verticalInput = Input.GetAxis("Vertical");	
	}

	private void Steer()
	{
		steeringAngle = maxSteerAngle * horizontalInput;
		frontDriverW.steerAngle = steeringAngle;
		frontPassengerW.steerAngle = steeringAngle;
	}

	private void Accelerate()
	{
		frontDriverW.motorTorque = verticalInput * motorForce;
		frontPassengerW.motorTorque = verticalInput * motorForce;
	}

	private void UpdateWheelPose()
	{
		UpdateWheelPose(frontDriverW, frontDriverT);
		UpdateWheelPose(frontPassengerW, frontPassengerT);
		UpdateWheelPose(rearDriverW, rearDriverT);
		UpdateWheelPose(rearPassengerW, rearPassengerT);
	}

	private void UpdateWheelPose(WheelCollider collider, Transform transform)
	{
		Vector3 pos = transform.position;
		Quaternion quat = transform.rotation;

		collider.GetWorldPose(out pos, out quat);

		transform.position = pos;
		transform.rotation = quat;
	}
}

