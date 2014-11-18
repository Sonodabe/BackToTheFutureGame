using UnityEngine;
using System.Collections;

public class GUIAspectRatioScale : MonoBehaviour 
{
	public Vector2 scaleOnRatio1 = new Vector2(0.1f, 0.1f);
	private float widthHeightRatio;

	// Use this for initialization
	void Update () {
		SetScale();
	}

	void SetScale () {
		widthHeightRatio = (float)Screen.width/Screen.height;
		transform.localScale = new Vector3 (scaleOnRatio1.x, widthHeightRatio * scaleOnRatio1.y, 1);
	}
}