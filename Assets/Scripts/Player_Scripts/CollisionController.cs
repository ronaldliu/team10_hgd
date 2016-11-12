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

		case "Obstacle_Damage":
			player.applyDamage (10f);
			break;
		case "Saw":
			player.applyDamage (1f);
			break;
		case "Spike":
			player.applyDamage (1f);
			break;
		case "Lava":
			player.applyDamage (1f);
			break;
		case "Slime":
			player.jumpForce = 600f;
			break;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		player.resetAttributesToDefault ();
	}
}
