using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	public GUITexture overlay;
	
	public GUIText scoreGUI;		// Object used to display the score
	public string scoreText;		// The text to display with the score

	public GUIText trickScoreGUI;
	public GUIText timeGUI;			// Object used to display the time
	
	public GUIText finalScoreGUI;
	public GUIText gameOverGUI;
	public string gameOverText;


	void Awake() {
		// Ensure the manager is not destroyed
		Object.DontDestroyOnLoad(gameObject);
	}

	public void UpdateOverlayAlpha(float alpha) {
		Color newColor = new Color(overlay.color.r, overlay.color.g, overlay.color.b, alpha);
		overlay.color = newColor;
	}

	public void UpdateScoreGUI(float score) {
		scoreGUI.text = scoreText + score;
	}

	public void UpdateTrickScoreGUI(float trickScore) {
		trickScoreGUI.text = "" + trickScore;
	}

	public void UpdateTimeGUI(string timeText) {
		timeGUI.text = timeText;
	}

	public void DisableGUI() {
		GUIText[] allGUI = GetComponentsInChildren<GUIText>();
		
		for (int i = 0; i < allGUI.Length; i++) {
			allGUI[i].enabled = false;
		}
	}

	public void EnableGameGUI() {
		scoreGUI.enabled = true;
		trickScoreGUI.enabled = true;
		timeGUI.enabled =true;
	}

	public void EnableFinalScoreGUI(float finalScore) {
		finalScoreGUI.text = "" + finalScore;
		gameOverGUI.text = gameOverText;
		
		finalScoreGUI.enabled = true;
		gameOverGUI.enabled = true;
	}
}
