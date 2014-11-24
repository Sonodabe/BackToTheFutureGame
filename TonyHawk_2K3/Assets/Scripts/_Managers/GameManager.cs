using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	// Enum used to track the current state of the game
	public enum GameState {
		Transition,
		BoardSelect,
		GameSetup,
		GameOn,
		FinalScore,
		GameOver,
		GameReset,
	}
	public static GameState state;

	public GameObject boardPrefab;
	public GameObject boardCamPrefab;

	private BoardController boardCont;
	private BoardCamera boardCam;
	private Transform spawnLoc;

	public GUIManager guiManager;

	private int score;				// The current score
	private int oldScore;			// The previous score
	public int scoreRate;			// The rate at which the score updates

	public int gameTime;			// The length of the game in seconds
	private float timer = 0.0f;		// Tracks the time that has passed


	void Awake () {
		// Ensure the manager is not destroyed
		Object.DontDestroyOnLoad(gameObject);
	}

	void Start () {
		// Set the default game state
		guiManager.DisableGUI();
		state = GameState.BoardSelect;

		Screen.lockCursor = true;
	}

	void FixedUpdate () {
		switch(state) {
		case GameState.BoardSelect:
			if (Input.GetAxis("Mouse ScrollWheel") < 0) {
				// Change to the main scene
				StartCoroutine(ChangeScene("Main_00", GameState.GameSetup, 2.0f));
			}

			break;
		case GameState.GameSetup:
			// Set up the game
			SetUpGame();

			break;
		case GameState.GameOn:
			// Update the time and score
			UpdateGame();

			break;
		case GameState.FinalScore:
			// Enable the final score
			guiManager.DisableGUI();
			guiManager.EnableFinalScoreGUI(score);

			// Transition to GameOver
			state = GameState.GameOver;

			break;
		case GameState.GameOver:
			// Transition to GameSetup
			if (Input.GetButtonDown("Reset")) {
				state = GameState.GameReset;
			}

			break;
		case GameState.GameReset:
			// Reset the game
			ResetGame();

			break;
		}
	}

	public void UpdateScore(int newScore) {
		if (state == GameState.GameOn) {
			// Update the score
			score += newScore;
		}
	}

	void SetUpGame() {
		// Find the spawn location
		spawnLoc = GameObject.FindGameObjectWithTag("SpawnLocation").transform;

		// Instantiate the board
		GameObject board = GameObject.Instantiate(boardPrefab, spawnLoc.position, spawnLoc.rotation) as GameObject;
		boardCont = board.GetComponent<BoardController>();
		boardCont.GetComponentInChildren<BoardTrick>().trickScoreGUI = guiManager.trickScoreGUI;

		// Instantiate the camera
		GameObject cam = GameObject.Instantiate(boardCamPrefab, spawnLoc.position, spawnLoc.rotation) as GameObject;
		boardCam = cam.GetComponent<BoardCamera>();
		boardCam.SetTarget(boardCont.transform);

		// Reset the game
		ResetGame();

		// Transition to GameOn
		state = GameState.GameOn;
	}

	void UpdateGame() {
		// Update the time remaining
		timer = Mathf.Max(timer - Time.fixedDeltaTime, 0.0f);
		
		// Calculate the minutes and seconds
		int minutes = ((int)timer / 60);
		int seconds = ((int)timer % 60);
		
		// Update the time GUI text
		string timeText = (minutes < 10) ? "0" + minutes : "" + minutes;
		timeText += (seconds < 10) ? ":0" + seconds : ":" + seconds;
		guiManager.UpdateTimeGUI(timeText);
		
		// Update the score GUI text
		if (oldScore != score) {
			oldScore = (oldScore + scoreRate < score) ? oldScore + scoreRate : score;
			guiManager.UpdateScoreGUI(oldScore);
		}

		/** PREVENT FALLING -- REMOVE EVENTUALLY **/
		if (boardCont.transform.position.y < -10) {
			boardCont.transform.position = spawnLoc.position;
			boardCont.transform.rotation = spawnLoc.rotation;
			boardCont.rigidbody.velocity = Vector3.zero;
		}

		// Transition to FinalScore
		if (timer == 0.0f && boardCont.GetIsGrounded()) {
			state = GameState.FinalScore;
		}
	}

	void ResetGame() {
		// Enable the game GUI
		guiManager.DisableGUI();
		guiManager.EnableGameGUI();
		
		// Enable the board
		boardCont.transform.position = spawnLoc.position;
		boardCont.transform.rotation = spawnLoc.rotation;
		boardCont.rigidbody.velocity = Vector3.zero;
		boardCont.enabled = true;
		
		// Reset the score
		guiManager.UpdateScoreGUI(0);
		score = oldScore = 0;
		
		// Initialize the timer
		timer = gameTime;
		
		// Transition to GameOn
		state = GameState.GameOn;
	}

	IEnumerator ChangeScene(string newScene, GameState newState, float transitionTime = 0.0f) {
		state = GameState.Transition;
		float timer = 0.0f;

		/** FADE THE CURRENT SCENE OUT **/
		while (timer < transitionTime) {
			timer += Time.deltaTime;

			// Fade out the scene
			float alpha = Mathf.Lerp (0.0f, 1.0f, timer / transitionTime);
			guiManager.UpdateOverlayAlpha(alpha);

			yield return null;
		}

		/** LOAD THE NEW SCENE **/
		Application.LoadLevel(newScene);
		state = newState;
		timer = 0.0f;

		/** FADE THE NEW SCENE IN **/
		while (timer < transitionTime) {
			timer += Time.deltaTime;

			// Fade in the scene
			float alpha = Mathf.Lerp (1.0f, 0.0f, timer / transitionTime);
			guiManager.UpdateOverlayAlpha(alpha);

			yield return null;
		}
	}
}
