using UnityEngine;
using System.Collections;

public class Charge : MonoBehaviour {

	public float maxDistance;	// Maximum distance the charge reaches
	public float minDistance;	// Minimum distance the charge reaches

	public float maxCharge = 3.0f;

	private float delta;		// The amount added to the maxCharge
	public float minCharge = 1.5f;
	public float decayRate = 0.1f; // How quickly the charge diminishes


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

	public void ChangeDelta(bool increase) {
		if(increase) {
			if ((maxCharge + delta) > minCharge) {
				delta -= decayRate;
				
				if (delta < minCharge - maxCharge) {
					delta = minCharge - maxCharge;
				}
			}
		} else {
			if (delta < 0) {
				if (Mathf.Abs((minCharge-maxCharge) - delta) < 0.02)
					delta = 5;

				delta += decayRate;
			}

			if (delta > 0) {
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
				float otherCharge = (other.maxCharge + other.delta);
				float charge = maxCharge + delta;
				// Calculate repulsion using Coulomb's Law
				Vector3 repulsion = transform.up * (charge * otherCharge) / (distance * distance);

				// Apply the repulsion to the root parent
				transform.root.rigidbody.AddForceAtPosition(repulsion, transform.position);
			}
		}
	}
}
