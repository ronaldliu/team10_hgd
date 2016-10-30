using UnityEngine;
using System.Collections;

public class SawScript : MonoBehaviour {
	
	public int turnSpeed = 2;
	private GameObject saw_sprite;

	void Start()
	{
		saw_sprite = transform.Find ("saw_sprite").gameObject;
	}

	// Update is called once per frame
	void Update () {
		saw_sprite.transform.Rotate (-Vector3.forward, 14 * Time.deltaTime * turnSpeed);
	}

	void OnTriggerEnter2D(Collider2D other){
		//print("this is working");
	}
}
