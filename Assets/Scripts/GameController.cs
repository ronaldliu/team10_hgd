using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	public Text scoreText;
	public Text timerText;
	public Text roundText;
	public Text ammoText;
	public Text countdownText;
	private float countdownTimer;
	public Text powerUpText;
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
	//Increase this for a longer Creator phase
	private float timer = 10.0F;
	private int state;
	private int round;

	private float width;
	private float startMaxXPos;
	private string[] phaseSwitchMessages = { "Time's Up!", "Get Ready...", "3", "2", "1", "Go!" };
	private float[] phaseSwitchTimes = { 1f, 2f, 0.5f, 0.5f, 0.5f, 0.5f };
	private int phaseSwitchState = 0;

	public CreatorController creatorPrefab;
	public PlayerController playerPrefab;
	private CreatorController creator;
	private PlayerController player;
	private WeaponController shoot;
	private DynamicCamera camera;

	void Start () {
		scoreText.text = "";
		ammoText.text = "";
		healthBar.parent.gameObject.SetActive (false);
		state = 0;
		round = 1;
		width = healthBar.rect.width;
		startMaxXPos = healthBar.offsetMax.x;
		//Create the creator
		creator = Instantiate (creatorPrefab);
		player = playerPrefab;
		camera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();
	}

	void Update () {
		switch (state) {
		case 0: //Creator
			{
				updateTimer (true);
				if (timer <= 0) {
					DestroyObject (creator.gameObject);
					timer = phaseSwitchTimes[0];
					countdownText.text = phaseSwitchMessages [0];
					state = 1;
				}
				break;
			}
		case 1: //Phase Switch
			{
				updateTimer (false);
				if (timer <= 0) {
					phaseSwitchState++;
					if (phaseSwitchState >= phaseSwitchMessages.Length) {
						countdownText.text = "";
						createPlayer ();
						timer = 120.0F;
						state = 2;
					} else {
						countdownText.text = phaseSwitchMessages [phaseSwitchState];
						timer = phaseSwitchTimes [phaseSwitchState];
					}
				}
				break;
			}
		case 2: //Player
			{
				updateTimer (true);
				// Displays current statistics for player in UI labels:
				scoreText.text = score.ToString ();
				ammoText.text = player.currentWeapon.ammo.ToString ();
				if (timer <= 0 || player.currentHealth <= 0) {
					state = 3;
				}
				break;
			}
		case 3: //TODO: End of Round
			{
				// Removes everything from UI:
				scoreText.text = "";
				timerText.text = "";
				roundText.text = "";
				ammoText.text = "";
				countdownText.text = "Round over!";
				break;
			}
		}
		roundText.text = "Round: " + round;
	}

	private void createPlayer() {
		player = Instantiate (playerPrefab);
		camera.setFollowing (player.gameObject);
		healthBar.parent.gameObject.SetActive (true);
	}

	private void updateTimer(bool showText) {
		timer = timer - Time.deltaTime;
		if (showText){
			timerText.text = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);

			roundText.text = "Round: " + round;
			// ammoText.text = player.currentWeapon.ammo.ToString();

			if (player.powerUpTimer > 0) {
				player.powerUpTimer = player.powerUpTimer - Time.deltaTime;
				powerUpText.text = player.powerUpName + " " + (int)((player.powerUpTimer + 1) / 60) + ":" + (int)(((player.powerUpTimer + 1) % 60) / 10) + (int)(((player.powerUpTimer + 1) % 60) % 10); 
			} else {
				powerUpText.text = "";
			}

		} else {
			
			// Removes everything from UI:
			scoreText.text = "";
			timerText.text = "";
			roundText.text = "";
			ammoText.text = "";
			powerUpText.text = "";
			timerText.text = "";
		}

	}

	public void updateHealth () {
		float playerHealth = player.currentHealth / player.startingHealth;
		healthBar.localScale = new Vector2 (Mathf.Clamp(playerHealth, 0F, 1F), healthBar.localScale.y);
	}
}
