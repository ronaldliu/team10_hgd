using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

	private float countdownTimer;

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
	public string[] phaseSwitchMessages = { "Time's Up!", "Get Ready...", "3", "2", "1", "Go!" };
	public float[] phaseSwitchTimes = { 1f, 2f, 0.5f, 0.5f, 0.5f, 0.5f };
	private int phaseSwitchState = 0;

	public CreatorController creatorPrefab;
	public PlayerController playerPrefab;
	private CreatorController creator;
	private PlayerController player;
	private DynamicCamera camera;

	void Start () {
		state = 0;
		round = 1;
		timer = 10f;
		camera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();
		generateMap ();
	}

	void Update () {
		switch (state) {
		case 0: //Creator
			{
				// JESSE ATTENTION!!!: THIS IS HOW WE ARE UPDATING THE UI SINCE THEY ARE SEPARATE NOW.
				//	IF THIS IS AS TERRIBLE AS I AM BEGINNING TO THINK MAYBE CHANGE PLZ.
				Text timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				if (!creator) {
					createCreator ();
				}
				creator.ui.updateTimers (timeText);

				if (timer <= 0) {
					DestroyObject (creator.gameObject);
					timer = phaseSwitchTimes[0];
					state = 1;
				}
				break;
			}
		case 1: //Phase Switch
			{
				if (timer <= 0) {
					phaseSwitchState++;
					if (phaseSwitchState >= phaseSwitchMessages.Length) {
						if (!player)
							createPlayer ();
						timer = 120.0F;
						state = 2;
					} else {
						timer = phaseSwitchTimes [phaseSwitchState];
					}
				}
				break;
			}
		case 2: //Player
			{
				if (timer <= 0 || player.currentHealth <= 0) {
					state = 3;
				}
				break;
			}
		case 3: //TODO: End of Round
			{
				break;
			}
		}
	}

	private void createPlayer() {
		player = Instantiate (playerPrefab);
		camera.setFollowing (player.gameObject);
	}

	private void createCreator() {
		creator = Instantiate (creatorPrefab);
		camera.setFollowing (creator.gameObject);
	}

	public void generateMap(){
		string rnd = Random.Range (1, 2).ToString();
		string mapPath = "Map" + rnd;
		GameObject map = Instantiate (Resources.Load(mapPath, typeof(GameObject))) as GameObject;
	}
}
