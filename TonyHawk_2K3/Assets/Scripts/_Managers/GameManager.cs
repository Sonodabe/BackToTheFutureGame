using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private delegate void GameState();

	// Gets called every fixed update
	// Will change depending on what state the game is in
	private GameState UpdateScene;
	
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
		UpdateScene = SelectBoard;

		Screen.lockCursor = true;
	}

	void FixedUpdate () {
		UpdateScene ();
	}

	public void UpdateScore(int newScore) {
		if (UpdateScene == UpdateGame) {
			// Update the score
			score += newScore;
		}
	}

	void Transition() {
		// Do anything we want to do between scenes
	}

	void SelectBoard() {
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
						// Change to the main scene
						StartCoroutine (ChangeScene ("Main_00", SetupGame, 2.0f));
		}
	}

	void SetupGame() {
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
		UpdateScene = UpdateGame;
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
			UpdateScene = ShowFinalScore;
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
		UpdateScene = UpdateGame;
	}

	void ShowFinalScore() {
		// Enable the final score
		guiManager.DisableGUI();
		guiManager.EnableFinalScoreGUI(score);
		
		// Transition to GameOver
		UpdateScene = GameOver;
	}

	void GameOver() {
		// Transition to GameSetup
		if (Input.GetButtonDown("Reset")) {
			UpdateScene = ResetGame;
		}

	}

	IEnumerator ChangeScene(string newScene, GameState newUpdate, float transitionTime = 0.0f) {
		UpdateScene = Transition;
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
		UpdateScene = newUpdate;
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
