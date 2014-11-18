using UnityEngine;
using System.Collections;

public class SelectionBoardController : MonoBehaviour {
    public float oscillationSpeed = 0.1f;  // The speed at which the board moves up and down
    public float amplitude = 3.0f; // The maximum distance the board moves up and down
    public float rotationSpeed = 0.8f;

    private float theta;
    private float lastY;

    // Use this for initialization
    void Start () {
        theta = 0;
        lastY = 0;
    }

    public void Activate () {
        theta += oscillationSpeed;
        float newY = amplitude * Mathf.Sin (theta);
        float deltaY = newY - lastY;
        
        transform.Translate (deltaY * transform.up);
        
        lastY = newY;

        float rotationX = -Input.GetAxis ("Mouse X");
        float rotationY = 0*Input.GetAxis ("Mouse Y");
        transform.Rotate (new Vector3(rotationY, rotationX, 0));
    }
}
