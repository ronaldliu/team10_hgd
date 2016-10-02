using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {

	protected PlayerController playerCont;
	public WeaponController attachedWeapon;

	private SpriteRenderer renderer;

	void Start() {
		renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = attachedWeapon.GetComponent<SpriteRenderer> ().sprite;
	}

	void OnTriggerEnter2D(Collider2D col) 
	{
		if (col.gameObject.tag == "Player")
			playerCont = col.gameObject.GetComponent<PlayerController> ();

		playerCont.setCurrentWeapon(Instantiate(attachedWeapon));
		Destroy (gameObject);
	}
}
