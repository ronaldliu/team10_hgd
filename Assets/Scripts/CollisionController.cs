using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	public PlayerController player;

	void Start() {
		player = GetComponent<PlayerController> ();
	}

	//For objects that aren't triggers
	void OnCollisionStay2D(Collision2D other) {
		switch (other.gameObject.tag) {

		case "LaserBeam":
			player.applyDamage (1f);
			break;
		}
	}

	//For objects set as triggers
	void OnTriggerStay2D(Collider2D other) {
		switch (other.gameObject.tag) {

		case "Spike":
			player.applyDamage (1f);
			break;
		case "Saw":
			player.applyDamage (1f);
			break;
		}
	}
}
