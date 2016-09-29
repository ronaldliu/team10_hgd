using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Text scoreText;
	public Text timerText;
	public Text roundText;
	public Text ammoText;
	/* 
	 * This is the rect transform for the green health because that is what
	 * 	we are modifying
	 */
	public RectTransform healthBar;


	private int score = 0;
	private float timer = 120.0F;
	private int round;

	private float width;
	private float startMaxXPos;

	public PlayerController player;
	private WeaponController shoot;

	void Start () {
		round = 1;
		width = healthBar.rect.width;
		startMaxXPos = healthBar.offsetMax.x;
	}

	void Update () {
		if (timer >= 0 && player.currentHealth > 0) {
			// Displays current statistics for player in UI labels:
			scoreText.text = score.ToString();
			timer = timer - Time.deltaTime;
			timerText.text = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
			roundText.text = "Round: " + round;
			ammoText.text = player.currentWeapon.ammo.ToString();
		} else {
			
			// Removes everything from UI:
			scoreText.text = "";
			timerText.text = "";
			roundText.text = "";
			ammoText.text = "";
		}
	}

	public void updateHealth () {
		float playerHealth = player.currentHealth / player.startingHealth;
		healthBar.localScale = new Vector2 (Mathf.Clamp(playerHealth, 0F, 1F), healthBar.localScale.y);

		if (player.currentHealth == 0)
			Application.LoadLevel(Application.loadedLevel);
	}
}
