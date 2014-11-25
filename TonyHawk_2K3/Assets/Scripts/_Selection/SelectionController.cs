using UnityEngine;
using System.Collections;

public class SelectionController : MonoBehaviour {

	private GameManager gameManager;

	public GameObject[] boardPrefabs;
	private SelectionBoardController[] boards;

	public Transform camPivot;
	private Quaternion boardRot;
	private int boardIndex;

	public float radius;
	private float arcDist;

	public float turnSpeed;
	public AudioClip turnSound;
	public float turnSoundVolume = 1.0f;
	public float correctionSpeed;


	void Start () {
		// Set the game manager
		gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));

		boards = new SelectionBoardController[boardPrefabs.Length];
		arcDist = 6.28f / boards.Length;

		for (int i = 0; i < boards.Length; i++) {
			float angle = arcDist * i;
			Vector3 pos = new Vector3(radius * Mathf.Sin(angle), 0.0f, radius * Mathf.Cos(angle));
			Quaternion rot = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.up);

			// Create and place the board
			GameObject board = GameObject.Instantiate(boardPrefabs[i], camPivot.position + pos, rot) as GameObject;
			board.transform.RotateAround(board.transform.position, board.transform.right, -90.0f);
			board.transform.parent = transform;

			// Modify the board components
			if (Application.isPlaying) {
				board.rigidbody.useGravity = false;
				Destroy(board.GetComponentInChildren<BoardController>());	// Destroy the standard BoardController
				Destroy(board.GetComponentInChildren<BoardTrick>());		// Destroy the BoardTrick

				ParticleSystem[] systems = board.GetComponentsInChildren<ParticleSystem>();

				// Destroy all ParticleSystem components
				foreach (ParticleSystem system in systems) {
					Destroy(system);
				}
			}

			// Add the SelectionBoardController
			boards[i] = board.AddComponent<SelectionBoardController>();
		}

		// Set the default board index
		boardRot = Quaternion.LookRotation(boards[0].transform.position - camPivot.position);
		boardIndex = 0;

		// Update the selected board in the game manager
		gameManager.boardPrefab = boardPrefabs[boardIndex];
	}

	void FixedUpdate () {
		float turnInput = Input.GetAxisRaw("BoardTurn");

		Quaternion lookRotation = boardRot;
		float speed = correctionSpeed;

		/** IF THE PLAYER IS TURNING THE CAMERA **/
		if (turnInput != 0) {
			// Use the turn speed
			speed = turnSpeed;

			// Update the desired rotation
			lookRotation = camPivot.rotation * Quaternion.Euler(0.0f, turnInput * arcDist * Mathf.Rad2Deg, 0.0f);

			/** CHECK IF THE BOARD INDEX SHOULD BE UPDATED **/
			int nextIndex = GetRelativeBoardIndex((int)turnInput);
			Quaternion nextRot = Quaternion.LookRotation(boards[nextIndex].transform.position - camPivot.position);

			float currAngleDist = Quaternion.Angle(camPivot.rotation, boardRot);
			float nextAngleDist = Quaternion.Angle(camPivot.rotation, nextRot);

			if (nextAngleDist <= currAngleDist) {
				boards[boardIndex].StartCoroutine("Reset");
				boards[nextIndex].StopCoroutine("Reset");

				boardRot = nextRot;
				boardIndex = nextIndex;

				// Update the selected board in the game manager
				gameManager.boardPrefab = boardPrefabs[boardIndex];
				SoundUtils.playSound(gameObject, turnSound, turnSoundVolume);
			}
		}

		camPivot.rotation = Quaternion.Lerp (camPivot.rotation, lookRotation, speed * Time.deltaTime);
		boards[boardIndex].Rotate();
	}

	int GetRelativeBoardIndex(int distance) {
		if (boardIndex + distance < 0) {
			return boards.Length + (distance - boardIndex);
		}
		else if (boardIndex + distance >= boards.Length) {
			return 0 + distance - (boards.Length - boardIndex);
		}

		return boardIndex + distance;

	}
}
