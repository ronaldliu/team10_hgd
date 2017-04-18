using UnityEngine;
using System.Collections;

public class FanWindScript : MonoBehaviour {

	void OnTriggerStay2D(Collider2D other) {
		transform.parent.GetComponent<FanScript> ().OnTriggerStay2D (other);
	}


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
