using UnityEngine;
using System.Collections;

public class CreatorObjRenderer : MonoBehaviour {

	public bool colliding;

	// Use this for initialization
	void Start () {
		colliding = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Creator") || other.gameObject.layer == LayerMask.NameToLayer ("Enemies")) {
			colliding = true;
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Creator") || other.gameObject.layer == LayerMask.NameToLayer ("Enemies")) {
			colliding = true;
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if(!GetComponent<Collider2D> ().IsTouchingLayers (LayerMask.GetMask ("Creator", "Enemies")))
			colliding = false;
	}

	void OnTriggerExit2D(Collider2D other) {
		if(!GetComponent<Collider2D> ().IsTouchingLayers (LayerMask.GetMask ("Creator", "Enemies")))
			colliding = false;
	}
}
