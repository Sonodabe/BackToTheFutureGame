using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BoardCamera : PivotBasedCameraRig {
	
	[SerializeField] private float moveSpeed = 10.0f;
	[SerializeField] private float turnSpeed = 2.5f;
	[SerializeField] private float turnSpeedLimit = 120.0f;
	[SerializeField] private float smoothTurnTime = 1.0f;

	// The velocity range at which the trick camera effect is minimized and maximized
	[SerializeField] private Vector2 trickCamRange = new Vector2(-7.5f, -15.0f);
	
	private float lastFlatAngle;				// The last turn angle of the camera target
	private float turnVelChange;				// Dummy turn velocity variable for SmoothDamp
	private float currentTurnAmount = 1.0f;		// How much the camera should turn toward the target
	private Vector3 lastCamPosition;

	
	protected override void Start () {
		base.Start ();
	}
	
	protected override void FollowTarget(float deltaTime) {
		if (!(deltaTime > 0) || target == null) return;

		Vector3 trickForward = (target.position - lastCamPosition).normalized;
		Vector3 targetForward = target.forward;

		// Get the amount the target has turned
		float currentFlatAngle = Mathf.Atan2(targetForward.x,targetForward.z) * Mathf.Rad2Deg;

		// Normalize the fall velocity based on the trick camera range
		float normFall = target.rigidbody.velocity.y - trickCamRange.x;

		/** DETERMINES HOW MUCH THE CAMERA SHOULD TURN **/
		if (turnSpeedLimit > 0 && Application.isPlaying) {
			float targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(lastFlatAngle, currentFlatAngle)) / deltaTime;
			
			// Calculate the percentage of the target turn speed the camera should match
			// If the target is turning faster than the turn limit, the camera matches 0%
			float desiredTurnAmount = Mathf.InverseLerp(turnSpeedLimit, turnSpeedLimit * 0.75f, targetSpinSpeed);
			float turnReactSpeed = (currentTurnAmount > desiredTurnAmount) ? 0.1f : smoothTurnTime;

			if (normFall > 0.0f) {
				currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, desiredTurnAmount, ref turnVelChange, turnReactSpeed);
				lastCamPosition = cam.position;
			}
			// If the trick camera is enabled
			else currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, 1.0f, ref turnVelChange, smoothTurnTime);
		}

		lastFlatAngle = currentFlatAngle;

		// Calculate the weight of the trick forward vector
		float trickWeight = Mathf.Lerp(0.0f, 1.0f, normFall / (trickCamRange.y - trickCamRange.x));

		// Calculate the rotation vector
		trickForward *= trickWeight;
		targetForward *= (1.0f - trickWeight);
		Quaternion lookRotation = Quaternion.LookRotation(targetForward + trickForward, Vector3.up);
		
		// Update the camera rotation and position
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
		pivot.rotation = Quaternion.Lerp(pivot.rotation, lookRotation, turnSpeed * currentTurnAmount * deltaTime);
	}
}