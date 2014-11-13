using UnityEngine;
using System.Collections;

public class Charge : MonoBehaviour {

	public float maxDistance;	// Maximum distance the charge reaches
	public float minDistance;	// Minimum distance the charge reaches

	public float charge = 3.0f;

	void Start () {
		// Identify the object as charged
		transform.tag = "Charged";
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
				float distance = Mathf.Max(hitInfo.distance, minDistance);
				float otherCharge = hitInfo.collider.GetComponent<Charge>().charge;

				// Calculate repulsion using Coulomb's Law
				Vector3 repulsion = transform.up * (charge * otherCharge) / (distance * distance);

				// Apply the repulsion to the root parent
				transform.root.rigidbody.AddForceAtPosition(repulsion, transform.position);
			}
		}
	}
}
