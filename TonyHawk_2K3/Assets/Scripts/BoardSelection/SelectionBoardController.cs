using UnityEngine;
using System.Collections;

public class SelectionBoardController : MonoBehaviour {
    public float oscillationSpeed = 0.1f;
    public float amplitude = 3.0f;
    public float rotationSpeed = 0.8f;
    private float theta;
    private float lastY;
    private bool oscillate;

    // Use this for initialization
    void Start () {
        theta = 0;
        lastY = 0;
        oscillate = false;
    }

    public void Oscillate (int caller) {
        Debug.Log (caller);
        oscillate = true;
    }

  
    // Update is called once per frame
    void FixedUpdate () {
        if (oscillate) {
            theta += oscillationSpeed;
            float newY = amplitude * Mathf.Sin (theta);
            float deltaY = newY - lastY;

            transform.Translate (deltaY * transform.up);

            lastY = newY;
            oscillate = false;
        }
    }
}
