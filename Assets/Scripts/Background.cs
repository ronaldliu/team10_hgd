using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {



	// Use this for initialization
	void Start () {
		Camera cam = Camera.main;
		float height = 2f * cam.orthographicSize;
		float width = height * cam.aspect;

		transform.localScale = new Vector3 (width, height, 1);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
