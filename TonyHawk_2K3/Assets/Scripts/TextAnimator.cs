using UnityEngine;
using System.Collections;

public class TextAnimator : MonoBehaviour {

	public float startScale;	// The starting size of the text
	public float endScale;		// The ending size of the text

	public float textTime;		// The overall time before fading
	public float growTime;		// The time for the text to grow
	public float fadeTime;		// Time to fade out

	private float timer;		// Object timer
	private TextMesh mesh;


	// Use this for initialization
	void Start () {
		// Get the text mesh component
		mesh = GetComponent<TextMesh>();

		// Set the initial text properties
		transform.localScale = new Vector3(startScale, startScale, startScale);
	}

	void FixedUpdate () {
		timer += Time.fixedDeltaTime;

		// Update the scale of the text
		if (growTime - timer > 0) {
			float scale = Mathf.Lerp(endScale, startScale, (growTime - timer) / growTime);
			transform.localScale = new Vector3(scale, scale, scale);
		}

		// Update the mesh transparency
		if (timer > textTime) {
			float alpha = Mathf.Lerp (0.0f, 1.0f, (textTime + fadeTime - timer) / fadeTime);
			mesh.color = new Color(mesh.color.r, mesh.color.g, mesh.color.b, alpha);
		}

		// Destroy the object if enough time has passed
		if (timer > textTime + fadeTime) {
			Destroy(gameObject);
		}
	}
}
