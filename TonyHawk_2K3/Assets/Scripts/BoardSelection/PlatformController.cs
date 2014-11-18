using UnityEngine;
using System.Collections;

public class PlatformController : MonoBehaviour {
    public float rotationSpeed = 1.0f; // The speed at which the platform rotates
    public GameObject[] prefabs;
    private SelectionBoardController[] boardControllers;
    private int controller_index;

    void Start () {
        boardControllers = new SelectionBoardController[prefabs.Length];
        for (int i = 0; i < prefabs.Length; i++) {
            float angle = 6.28f * i / prefabs.Length;
            Vector3 position = new Vector3 (0.4f * Mathf.Sin (angle), 65, 0.4f * Mathf.Cos (angle));
            Quaternion rotation = Quaternion.AngleAxis (Mathf.Rad2Deg * angle, Vector3.up);

            GameObject board = Instantiate (prefabs [i], Vector3.zero, rotation) as GameObject;

            board.transform.parent = transform;
            board.transform.localPosition = position;

            // Create separate prefab of this size
            board.transform.localScale = new Vector3 (2.14f, 1250f, 0.02f);

            SelectionBoardController boardController = board.AddComponent<SelectionBoardController> ();
            boardControllers [i] = boardController;
        }

        controller_index = 0;
    }
  
    // Update is called once per frame
    void FixedUpdate () {
        float boardTurn = Input.GetAxis ("BoardTurn");
        if (boardTurn != 0) {
            transform.Rotate (-boardTurn * rotationSpeed * transform.up);

            // Calculate the current board
            int rotation = 360 - (int)transform.localRotation.eulerAngles.y;
            // Did some math, this will give you to which board the platform is angled
            controller_index = ((1 + (rotation * prefabs.Length / 180)) / 2) % prefabs.Length;
        }
  
        boardControllers [controller_index].Activate ();
    }
}
