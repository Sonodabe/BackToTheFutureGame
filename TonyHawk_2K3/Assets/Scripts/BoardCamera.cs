using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BoardCamera : PivotBasedCameraRig {

	[SerializeField] private float moveSpeed = 3.0f;
	[SerializeField] private float turnSpeed = 1.0f;
	[SerializeField] private float turnSpeedLimit = 90.0f;
	[SerializeField] private float smoothTurnTime = 1.0f;
	
	private float lastFlatAngle;				// The last turn angle of the camera target
	private float currentTurnAmount = 1.0f;		// How much the camera should turn toward the target
	private float turnSpeedVelocityChange;		// Dummy turn velocity variable for SmoothDamp


	protected override void Start () {
		base.Start ();
	}
	
	protected override void FollowTarget(float deltaTime) {

		if (!(deltaTime > 0) || target == null) return;

		// Get the forward direction of the camera target
		Vector3 targetForward = target.forward;

		// Get the turn angle of the camera target
		float currentFlatAngle = Mathf.Atan2(targetForward.x,targetForward.z) * Mathf.Rad2Deg;

		if (turnSpeedLimit > 0) {
			// Calculate the turn speed
			float targetSpinSpeed = Mathf.Abs(Mathf.DeltaAngle(lastFlatAngle,currentFlatAngle)) / deltaTime;

			// Calculate the percentage of the target turn speed the camera should match
			// If the target is spinning faster than the turn limit, the camera matches 0%
			float desiredTurnAmount = Mathf.InverseLerp(turnSpeedLimit, turnSpeedLimit * 0.75f, targetSpinSpeed);

			// Determine how quickly the speed percentage affects the camera turn speed
			float turnReactSpeed = (currentTurnAmount > desiredTurnAmount) ? 0.1f : smoothTurnTime;

			if (Application.isPlaying) {
				// Gradually change the amount the camera turns to the desired amount
				currentTurnAmount = Mathf.SmoothDamp(currentTurnAmount, desiredTurnAmount,
				                                     ref turnSpeedVelocityChange, turnReactSpeed);
			}
			// Because SmoothDamp does not work in editor mode
			else currentTurnAmount = desiredTurnAmount;
		}

		// Update the last turn angle of the camera target
		lastFlatAngle = currentFlatAngle;

		// Calculate the angle of rotation to the camera target
		/** HANDLE TRICK CAMERA ROTATION HERE **/
		Quaternion lookRotation = Quaternion.LookRotation(targetForward, Vector3.up);

		// Update the camera rotation and position
		transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, turnSpeed * currentTurnAmount * deltaTime);
		transform.position = Vector3.Lerp(transform.position, target.position, deltaTime * moveSpeed);
	}
}
