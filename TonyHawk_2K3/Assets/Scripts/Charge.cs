using UnityEngine;
using System.Collections;

public class Charge : MonoBehaviour {

	public float maxDistance;	// Maximum distance the charge reaches
	public float minDistance;	// Minimum distance the charge reaches

	public float defaultCharge = 3.0f; // The resting charge of the object
	public float minCharge = 1.5f; // The lower bound of the charge
	public float jumpCharge = 5.5f; // The charge emitted when the object is released at min charge
	public float decayRate = 0.1f; // How quickly the charge diminishes

	private float delta;		// The amount added to the maxCharge


	void Start () {
		// Identify the object as charged
		transform.tag = "Charged";
		delta = 0;
	}

	void FixedUpdate () {
		// If the object is not static, then calculate repulsion
		if (!gameObject.isStatic) {
			Repulse();
		}
	}

	public void ChangeDelta(bool stress_system) {
		if(stress_system) {
			if (delta > minCharge - defaultCharge) {
				delta -= decayRate;
			} else {
				delta = minCharge - defaultCharge;
			}
		} else {
			if (delta < 0) {
				if (Mathf.Abs((minCharge-defaultCharge) - delta) < decayRate/10)
					delta = jumpCharge;

				delta += decayRate;
			} else {
				delta -= decayRate;
			}
		}
	}

	void Repulse () {
		RaycastHit hitInfo;

		if (Physics.Raycast (transform.position, -transform.up, out hitInfo, maxDistance)) {
			// Check whether the opposing object is charged
			if (hitInfo.collider.tag == "Charged") {
				float distance = Mathf.Max(hitInfo.distance, minDistance);
				Charge other = hitInfo.collider.GetComponent<Charge>();
				float otherCharge = (other.defaultCharge + other.delta);
				float charge = defaultCharge + delta;
				// Calculate repulsion using Coulomb's Law
				Vector3 repulsion = transform.up * (charge * otherCharge) / (distance * distance);

				// Apply the repulsion to the root parent
				transform.root.rigidbody.AddForceAtPosition(repulsion, transform.position);
			}
		}
	}
}
