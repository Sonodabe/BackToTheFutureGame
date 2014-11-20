using UnityEngine;
using System.Collections;

public class SelectionBoardController : MonoBehaviour {

	private Quaternion defRot;

	private float rotationSpeed = 6.0f;
	private float correctionSpeed = 2.0f;


	void Start () {
		// Record the default rotation
		defRot = transform.rotation;
	}
	
	public void Rotate() {
		float tiltZ = -Input.GetAxis("Mouse X");
		transform.Rotate(new Vector3(0.0f, 0.0f, tiltZ * rotationSpeed));
	}

	IEnumerator Reset() {
		while (transform.rotation != defRot) {
			// Rotate the board back to its default rotation
			transform.rotation = Quaternion.Lerp(transform.rotation, defRot, correctionSpeed * Time.deltaTime);

			yield return null;
		}
	}
}
