using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHud : MonoBehaviour {

	public Text scoreText;
	public Text timerText;
	public Text ammoText;
	public Text powerUpText;
	public Image powerUpImage;
	/* 
	 * This is the rect transform for the green health because that is what
	 * 	we are modifying
	 */
	public RectTransform healthBar;

	// To determine which controller is which player
	// This is for the purpose of swapping roles
	// public int whoIsPlayer;
	// public int whoIsCreator;

	private int score = 0;
	public float powerUpTimer;

	private PlayerController player;

	void Awake () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController> ();
		eraseAllText ();
		// healthBar.parent.gameObject.SetActive (false);
	}

	void Update() 
	{
		updateTimers ();
		ammoText.text = player.currentWeapon.ammo.ToString ();
	}

	private void updateTimers() {
		if (powerUpTimer > 0) powerUpTimer -= Time.deltaTime;

		if (powerUpTimer > 0) {
			powerUpText.text =	"" +
			(int)(((powerUpTimer + 1) % 60) / 10) +
			(int)(((powerUpTimer + 1) % 60) % 10);
			powerUpImage.color = new Color(255, 255, 255, 1);
		} else {
			powerUpText.text = "";
			powerUpImage.color = new Color(255, 255, 255, 0);
		}
	}

	public void updateTimers(string timeFromGame) {
		timerText.text = timeFromGame;
	}

	public void applyPowerUp(float duration, Sprite image)
	{
		GameObject.Find("PowerUpDisplay").GetComponentInChildren<Image>().sprite = image;
		powerUpTimer = duration;
	}

	// Update the UI's health to correctly reflect the current player's health
	public void updateHealth () {
		float playerHealth = player.currentHealth / player.startingHealth;
		healthBar.localScale = new Vector2 (Mathf.Clamp(playerHealth, 0F, 1F), healthBar.localScale.y);
	}

	public void updateScore (float score)
	{
		scoreText.text = ((int)score).ToString();
	}

	// PRIVATE FUNCTIONS
	private void eraseAllText()
	{
		scoreText.text = "";
		timerText.text = "";
		ammoText.text = "";
		powerUpText.text = "";
	}
}
