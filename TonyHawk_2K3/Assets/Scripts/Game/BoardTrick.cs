using UnityEngine;
using System.Collections;

public class BoardTrick : MonoBehaviour {

	// Reference to the game manager
	private GameManager gameManager;

	public float scoreAdjust;	// Adjusts the score
	public float minSpeed;		// The minimum angular speed for tricks

	public GUIText trickScoreGUI;	// The GUI associated with tricks
	public string trickScoreText;	// Text to print in the GUI
	public string multiplierText;	// Text to print the multiplier
	
	private float trickScore;		// The current total trick score

	public int maxMult = 8;			// Maximum value for the multiplier
	public int multRate = 1;		// The rate of multiplier increase per second
	private float timer;			// Tracks the time of the trick
	private int multiplier;			// Rounded multiplier

	public int highScore;			// Value considered to be a high score
	public Color lowScoreColor;		// Color associated with low scores
	public Color highScoreColor;	// Color associated with high scores

	public GameObject successText;		// Text mesh that appears on trick success
	public GameObject successPart;		// Particle effect that appears on trick success
	public AudioClip[] successAudio;	// List of clips that play on success
	

	// Use this for initialization
	void Start () {
		// Set the game manager
		gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));

		// Set the trick score GUI to empty
		trickScoreGUI.text = "";
		timer = 0.0f;
	}

	public void UpdateTrickScore() {
		// Get the angular speed of the object
		float angularSpeed = transform.root.rigidbody.angularVelocity.magnitude;

		if (angularSpeed > minSpeed) {
			// Calculate the new score
			float newScore = (Vector3.down + transform.up).sqrMagnitude * angularSpeed;

			// Add the new score to the trick score
			trickScore += (newScore * scoreAdjust);

			// Update the multiplier
			timer += Time.fixedDeltaTime;
			multiplier = (int) Mathf.Min((timer / multRate) + 1, maxMult);

			// Update the GUI
			trickScoreGUI.text = string.Format(
				"{0} {1} {2} {3}", trickScoreText, (int)trickScore, multiplierText, multiplier);
		}
	}

	public void ResetTrickScore() {
		// Reset the score
		trickScore = 0.0f;
		timer = 0.0f;

		// Reset the GUI
		trickScoreGUI.text = "";
	}

	public void UpdateScore() {
		// Calculate the trick score
		int newScore = (int) trickScore * multiplier;

		if (newScore > 0) {
			// Send a message to the game manager
			gameManager.UpdateScore(newScore);

			// Calculate the success percentage
			float successPerc = Mathf.Min((float)newScore / highScore, 1.0f);

			// Spawn the text mesh and particle effect
			SpawnTextMesh(newScore, successPerc);
			SpawnParticle(successPerc);

			// Plays success sound
			int clipIndex = (int)(successPerc * (successAudio.Length - 1));
			SoundUtils.playSound(this.gameObject, successAudio[clipIndex], 0.75f);
		}
	}

	void SpawnTextMesh(int score, float successPerc) {
		// Destroy existing success text meshes
		Transform oldText = transform.Find ("Success Trick Text");
		if (oldText) Destroy (oldText.gameObject);

		// Create a new text mesh
		GameObject newText = GameObject.Instantiate(successText) as GameObject;
		
		newText.transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 90.0f, 0.0f);
		newText.transform.position = transform.position;
		newText.transform.parent = transform;
		newText.name = "Success Trick Text";
		
		// Set the text scale based on the success percentage
		newText.GetComponent<TextAnimator>().endScale = Mathf.Lerp(0.5f, 1.5f, successPerc);
		
		// Set the text color and string
		TextMesh textMesh = newText.GetComponent<TextMesh>();
		textMesh.color = Color.Lerp(lowScoreColor, highScoreColor, successPerc);
		textMesh.text = "" + score;
	}

	void SpawnParticle(float successPerc) {
		// Create the particle effect
		var newPart = GameObject.Instantiate(successPart, transform.root.position, transform.rotation);
		((GameObject)newPart).transform.parent = transform.root;

		// Modify the emission based on the success percentage
		ParticleEmitter emitter = ((GameObject)newPart).GetComponent<ParticleEmitter>();
		emitter.maxEmission *= successPerc;
		emitter.maxEnergy *= successPerc;
	}
}
