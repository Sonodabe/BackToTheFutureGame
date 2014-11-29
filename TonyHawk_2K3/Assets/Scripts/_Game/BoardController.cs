using UnityEngine;
using System.Collections;

public class BoardController : MonoBehaviour {

	public float maxSpeed;	// The maximum board speed
	public float accelMod;	// Modifies the acceleration

	public float torqueZ;	// Modifies forward tilt
	public float torqueX;	// Modified sideways tilt

	public float turnForce;	// Determines turn force

	public float groundDist;	// Distance to determine whether grounded
	private bool isGrounded;	// Whether the board is grounded

	public float crouchForce;	// The amount of force applied to the board for crouching

	// Reference to the trick controller
	private BoardTrick boardTrick;
	private Charge[] charges;


	// Use this for initialization
	void Start () {
		// Get the trick controller
		boardTrick = GetComponentInChildren<BoardTrick>();
		charges = GetComponentsInChildren<Charge>();

		// Check if the board is grounded
		isGrounded = (Physics.Raycast(transform.position, -transform.up, groundDist)) ? true : false;
	}
	
	void FixedUpdate () {
		// Check the board trick status
		PerformTrick();

		// Get the speed of the board
		float speed = transform.InverseTransformDirection(rigidbody.velocity).z;
		speed = Mathf.Abs(speed);

		// Get the acceleration input
		float wheelInput = Input.GetAxis("Mouse ScrollWheel");

		// Update the velocity of the hoverboard
		if (isGrounded && wheelInput < 0 && (speed + wheelInput * accelMod) < maxSpeed) {
			rigidbody.velocity += transform.forward * -wheelInput * accelMod;
		}

		// Get the mouse delta input
		float tiltZ = -Input.GetAxis("Mouse X");
		float tiltX = Input.GetAxis("Mouse Y");

		// Apply tilting torques to the hoverboard
		rigidbody.AddRelativeTorque(tiltX * torqueX, 0.0f, tiltZ * torqueZ);

		// Get the turning input
		float turnInput = Input.GetAxisRaw("BoardTurn");

		// Apply turning torque to the hoverboard
		rigidbody.AddRelativeTorque(0.0f, turnForce * turnInput, 0.0f);
	}

	public void UpdateChargeEnabled(bool isEnabled) {
		// Toggle the enabled status for all charges
		for (int i = 0; i < charges.Length; i++) {
			charges[i].enabled = isEnabled;
		}
	}

	public bool GetIsGrounded() {
		return isGrounded;
	}

	void PerformTrick() {
		// Update the board grounded state
		bool prevGrounded = isGrounded;
		isGrounded = (Physics.Raycast(transform.position, -transform.up, groundDist)) ? true : false;

		// If the board was not grounded, update trick score
		if (!prevGrounded) {
			boardTrick.UpdateTrickScore();
		}

		// If the board landed a trick, update the total score
		if (!prevGrounded && isGrounded) {
			boardTrick.UpdateScore();
			boardTrick.ResetTrickScore();
		}
	}

	void OnCollisionStay(Collision collision) {
		// If the board collides with an object, then tricks should fail
		boardTrick.ResetTrickScore();
	}
}
