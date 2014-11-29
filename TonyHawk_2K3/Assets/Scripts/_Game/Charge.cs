using UnityEngine;
using System.Collections;

public class Charge : MonoBehaviour {

	public float maxDistance;	// Maximum distance the charge reaches
	public float minDistance;	// Minimum distance the charge reaches

	public float charge = 3.0f; // The resting charge of the object

	private Collider otherCollider;
	private Charge otherCharge;


	void Start () {
		// Identify the object as charged
		Transform[] children = GetComponentsInChildren<Transform>();

		foreach (Transform child in children) {
			child.tag = "Charged";
		}
	}

	void FixedUpdate () {
		// If the object is not static, then calculate repulsion
		if (!gameObject.isStatic) {
			Repulse();
		}
	}

	void Repulse () {
		RaycastHit hitInfo;

		if (Physics.Raycast (transform.position, -transform.up, out hitInfo, maxDistance)) {
			// Check whether the opposing object is charged
			if (hitInfo.collider.tag == "Charged") {

				// Check if the collider has changed
				if (hitInfo.collider != otherCollider) {
					otherCollider = hitInfo.collider;
					otherCharge = otherCollider.GetComponentInParent<Charge>();
				}

				float distance = Mathf.Max(hitInfo.distance, minDistance);
				float repulsion = charge * otherCharge.charge;

				// Calculate repulsion using Coulomb's Law
				Vector3 repulseVec = transform.up * (repulsion) / (distance * distance);

				// Apply the repulsion to the root parent
				transform.root.rigidbody.AddForceAtPosition(repulseVec, transform.position);
			}
		}
	}
}
