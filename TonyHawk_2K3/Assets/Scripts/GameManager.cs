using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Enum used to track the current state of the game
	public enum GameState {
		MainMenu,
		BoardSelect,
		GameSetup,
		GameOn,
		FinalScore,
		GameOver,
		Leaderboard
	}
	public static GameState state;

	public BoardController boardCont;
	public Transform spawnLoc;

	private int score;
	private int oldScore;

	public GUIText scoreGUI;	// Object used to display the score
	public string scoreText;	// The text to display with the score
	public int scoreRate;		// The rate at which the score updates

	public int gameTime;		// The length of the game in seconds
	public GUIText timeGUI;		// Object used to display the time
	private float timer;

	public GUIText finalScoreGUI;
	public GUIText gameOverGUI;
	public string gameOverText;


	// Use this for initialization
	void Start () {
		// Set the default game state
		state = GameState.GameSetup;
	}

	void FixedUpdate () {
		switch(state) {
		case GameState.GameSetup:
			// Set up the game
			SetUpGame();

			// Transition to GameOn
			state = GameState.GameOn;

			break;
		case GameState.GameOn:
			// Update the time and score
			UpdateGameStats();

			// Transition to FinalScore
			if (timer == 0.0f && boardCont.GetIsGrounded()) {
				state = GameState.FinalScore;
			}

			break;
		case GameState.FinalScore:
			// Enable the final score
			EnableFinalScore();

			// Transition to GameOver
			state = GameState.GameOver;

			break;
		case GameState.GameOver:
			// Transition to GameSetup
			if (Input.GetButtonDown("Reset")) {
				state = GameState.GameSetup;
			}

			break;
		}

		/** PREVENT FALLING -- REMOVE EVENTUALLY **/
		if (boardCont.transform.position.y < -10) {
			boardCont.transform.position = spawnLoc.position;
			boardCont.transform.rotation = spawnLoc.rotation;
			boardCont.rigidbody.velocity = Vector3.zero;
		}
	}

	public void UpdateScore(int newScore) {
		if (state == GameState.GameOn) {
			// Update the score
			score += newScore;
		}
	}

	void ResetGUI() {
		GUIText[] allGUI = GetComponentsInChildren<GUIText>();

		for (int i = 0; i < allGUI.Length; i++) {
			allGUI[i].enabled = false;
		}
	}

	void SetUpGame() {
		ResetGUI();

		// Enable the game GUI
		scoreGUI.enabled = true;
		timeGUI.enabled = true;

		// Enable the board
		boardCont.transform.position = spawnLoc.position;
		boardCont.transform.rotation = spawnLoc.rotation;
		boardCont.rigidbody.velocity = Vector3.zero;
		boardCont.enabled = true;

		// Set the score GUI to the default text
		score = oldScore = 0;
		scoreGUI.text = scoreText + score;
		
		// Initialize the timer
		timer = gameTime;
	}

	void UpdateGameStats() {
		// Update the time remaining
		timer = Mathf.Max(timer - Time.fixedDeltaTime, 0.0f);
		
		// Calculate the minutes and seconds
		int minutes = ((int)timer / 60);
		int seconds = ((int)timer % 60);
		
		// Update the time GUI text
		string timeText = (minutes < 10) ? "0" + minutes : "" + minutes;
		timeText += (seconds < 10) ? ":0" + seconds : ":" + seconds;
		timeGUI.text = timeText;
		
		// Update the score GUI text
		if (oldScore != score) {
			oldScore = (oldScore + scoreRate < score) ? oldScore + scoreRate : score;
			scoreGUI.text = scoreText + oldScore;
		}
	}

	void EnableFinalScore() {
		ResetGUI();
		
		// Update and enable final GUI
		finalScoreGUI.text = "" + score;
		gameOverGUI.text = gameOverText;
		
		finalScoreGUI.enabled = true;
		gameOverGUI.enabled = true;
	}
}
