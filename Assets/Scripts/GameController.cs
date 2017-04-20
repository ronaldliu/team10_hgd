﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{

	private float countdownTimer;

	// To determine which controller is which player
	// This is for the purpose of swapping roles
	// public int whoIsPlayer;
	// public int whoIsCreator;

	public static int winScore = 0;
	private int[] scores = {0, 0};
	private int currPlayer;
	private int currCreator;
	//Increase this for a longer Creator phase
	private float timer = 10.0F;
	private int state;
	private int round;
	private bool ranTwice;
	private bool playerReachedEnd;
	private bool roundStarted;

	private float width;
	private float startMaxXPos;
	public string[] phaseSwitchMessages = { "Time's Up!", "Get Ready...", "3", "2", "1", "Go!" };
	public float[] phaseSwitchTimes = { 1f, 2f, 0.5f, 0.5f, 0.5f, 0.5f };
	private int phaseSwitchState = 0;

	public GameObject scoreboardCanvas;
	private Scoreboard scoreboard;
	public GameObject creatorPrefab;
	public GameObject playerPrefab;
	public GameObject playerEntPrefab;

	private Transform creatorContainer;
	private CreatorController creator;
	private CreatorHud creatorUI;
	private SpriteRenderer creatorRenderer;

	private Transform playerContainer;
	private PlayerController player;
	private PlayerHud playerUI;
	private SpriteRenderer[] playerSprites;

	private MapSelection mapSlectionUI;
	private bool selectingMap;

	public Color player1Color;
	public Color player2Color;
	public bool twoControllers;
	public int maxRounds;
	public bool randomMaps;

	public GameObject spawnedContainer;

	private GameObject mapContainer;
	private MapInfo mapinfo;
	private MatchSettingsHolder SettingsManager;

	new private DynamicCamera camera;

	public AudioSource createSource;
	public AudioSource runnerSource;
	public AudioSource menuSource;
	private bool startMusic;

	//New keywords is used to hide the default Unity camera keyword for this one.

	void Start()
	{
		state = 0;
		round = 1;
		timer = 90f;
		scores[0] = 0;
		scores[1] = 0;
		currPlayer = 0;
		currCreator = 1;
		ranTwice = false;
		playerReachedEnd = false;
		roundStarted = false;

		if (maxRounds <= 0)
			maxRounds = 5;

		spawnedContainer = transform.FindChild("spawnedContainer").gameObject;
		camera = GameObject.Find("Main Camera").GetComponent<DynamicCamera>();
		scoreboardCanvas = Instantiate(scoreboardCanvas);
		scoreboard = scoreboardCanvas.transform.FindChild("Scoreboard").GetComponent<Scoreboard>();
		scoreboardCanvas.SetActive(false);

		SettingsManager = GameObject.Find ("SettingsHolder").GetComponent<MatchSettingsHolder> ();
		player1Color = SettingsManager.player1Color;
		player2Color = SettingsManager.player2Color;
		twoControllers = SettingsManager.useTwoControllers;

		mapSlectionUI = GameObject.Find ("MapSelectionCanvas").GetComponent<MapSelection>();
		mapSlectionUI.gameObject.SetActive (false);
		selectingMap = false;

		maxRounds = SettingsManager.rounds;
		randomMaps = SettingsManager.randomMaps;
		
		//set up music objects
		createSource = GameObject.Find ("CreatorMusic").GetComponent<AudioSource>();
		runnerSource = GameObject.Find ("PlayerMusic").GetComponent<AudioSource>();
		menuSource = GameObject.Find ("MenuMusic").GetComponent<AudioSource>();
		startMusic = true;
		createSource.Play();
		AudioSource temp = GameObject.Find ("BackgroundMusic").GetComponent<AudioSource>();
		temp.Stop ();
	}

	void Update() {

		if (Input.GetButtonDown("Back_1"))
			SceneManager.LoadScene("FinalGame");

		switch (state)
		{
		case 0: //Creator
			{
				//start music
				if (startMusic) {
					menuSource.Pause();
					createSource.Play();
					startMusic = false;
				}

				if (!mapContainer) {
					generateMap ();
				} else {
					//TODO bad hotfix (also in plaer state)
					foreach(BoxCollider2D boxc in mapContainer.GetComponentsInChildren<BoxCollider2D>()) {
						boxc.usedByEffector = false;
					}
				}
				
				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				if (!creator)
				{
					createCreator();
					// Position creator at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = creator.transform.position.z;
					creator.transform.position = tempPos;
					creator.money = mapinfo.mapMoney;
					creator.ui.updateMoneyText(creator.money);
					if (currCreator == 0) {
						//creatorRenderer.color = player1Color;
						creatorRenderer.color = new Color(player1Color.r, player1Color.g, player1Color.b);
					} else {
						//creatorRenderer.color = player2Color;
						creatorRenderer.color = new Color(player2Color.r, player2Color.g, player2Color.b);
						//creatorRenderer.color = new Color(0f, 0f, 0f, 1f);
					}
				}
				creator.ui.updateTimers(timeText);

				if (timer <= 0 || creator.money <= 0)
				{
					creatorContainer.gameObject.SetActive(false);
					scoreboardCanvas.SetActive(true);
					scoreboard.updateScoreboardAll(
						phaseSwitchMessages[0], 
						scores[0], 
						scores[1], 
						currPlayer, 
						currCreator, 
						round.ToString(),
						"Starting Player Phase");
					phaseSwitchState = 0;
					timer = phaseSwitchTimes[0];
					nextState();
					//set bool for beginning of next round
					startMusic = true;
				}
				break;
			}
		case 1: //Phase Switch
			{
				//music handling
				if (startMusic) {
					createSource.Pause();
					menuSource.Play();
					startMusic = false;
				}

				if (timer <= 0)
				{
					phaseSwitchState++;
					if (phaseSwitchState >= phaseSwitchMessages.Length)
					{
						scoreboardCanvas.SetActive(false);
						if (!playerContainer)
							createPlayer();
						playerContainer.gameObject.SetActive(true);
						camera.setFollowing(player.gameObject);

						timer = mapinfo.timeToFinish;

						// Position player at start
						Vector3 tempPos = mapinfo.startLocation.transform.position;
						tempPos.z = player.transform.position.z;
						player.transform.position = tempPos;

						SentryController[] sentries = spawnedContainer.GetComponentsInChildren<SentryController>();
						foreach (SentryController sentry in sentries)
						{
							sentry.enabled = true;
							sentry.setPlayer();
						}

						nextState();
						//set bool for beginning of next round
						startMusic = true;
					}
					else
					{
						scoreboard.updateScoreboardMessage(phaseSwitchMessages[phaseSwitchState]);
						timer = phaseSwitchTimes[phaseSwitchState];
					}
				}
				break;
			}
		case 2: //Player
			{
				//start music
				if (startMusic) {
					menuSource.Pause();
					runnerSource.Play();
					startMusic = false;
				}

				if (!roundStarted) {
					//change color of player
					foreach (SpriteRenderer ob in playerSprites) {
						Debug.Log (ob.name);
						if (currPlayer == 0) {
							PlayerController playerController = player.GetComponent<PlayerController> ();
							playerController.defaultColor = new Color (player1Color.r, player1Color.g, player1Color.b);
							ob.color = new Color(player1Color.r, player1Color.g, player1Color.b);
						} else {
							PlayerController playerController = player.GetComponent<PlayerController> ();
							playerController.defaultColor = new Color(player2Color.r, player2Color.g, player2Color.b);
							ob.color = new Color(player2Color.r, player2Color.g, player2Color.b);
						}
					}
					foreach(BoxCollider2D boxc in mapContainer.GetComponentsInChildren<BoxCollider2D>()) {
						boxc.usedByEffector = true;
					}
					roundStarted = true;
				}
				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				player.ui.updateTimers(timeText);
				if (timer <= 0 || playerReachedEnd)
				{
					playerContainer.gameObject.SetActive(false);

					int cPlayerScore;
					int cCreatorScore;
					if (playerReachedEnd)
					{
						cPlayerScore = (int)((timer / mapinfo.timeToFinish) * 1000);
						cCreatorScore = 1000 - cPlayerScore;
					}
					else
					{
						cPlayerScore = 100;
						cCreatorScore = 400;
					}
					scores[currPlayer] += cPlayerScore;
					scores[currCreator] += cCreatorScore;

					// Swap the roles
					if (currPlayer == 0 && currCreator == 1)
					{
						currPlayer = 1;
						currCreator = 0;
					}
					else
					{
						currPlayer = 0;
						currCreator = 1;
					}

					if (twoControllers)
					{
						player.setController (currPlayer + 1);
						creator.setController (currCreator + 1);
					}
					else
					{
						player.setController (1);
						creator.setController (1);
					}

					// The string to Print above the scoreboard
					string information;

					if (ranTwice)
					{
						round++;
						ranTwice = false;
						information = "Starting Next Round";
						Destroy(mapContainer);
						if (!randomMaps) {
							selectingMap = true;
							mapSlectionUI.gameObject.SetActive (true);
						}
					}
					else
					{
						information = "Swapping Roles";
						ranTwice = true;
					}

					if (round > maxRounds)
					{
						information = "The Loser Is...";
						state = 4;
						mapSlectionUI.gameObject.SetActive (false);
						scoreboardCanvas.SetActive(true);
						scoreboard.updateScoreboardAll(
							phaseSwitchMessages[0], 
							scores[0], 
							scores[1], 
							currPlayer, 
							currCreator, 
							(round-1).ToString(),
							information);
						break;
					}

					scoreboardCanvas.SetActive(true);
					scoreboard.updateScoreboardAll(
						phaseSwitchMessages[0], 
						scores[0], 
						scores[1], 
						currPlayer, 
						currCreator, 
						round + "\\" + maxRounds,
						information);

					// How long to wait for swap phase
					timer = 10f;

					// Reset the player to starting
					player.startingHealth = 100f;
					player.resetEverything();
					nextState();
					//set bool for beginning of next round
					startMusic = true;
					playerReachedEnd = false;
					roundStarted = false;
				}
				break;
			}
		case 3: //Phase Switch
			{
				//start music
				if (startMusic) {
					runnerSource.Pause();
					menuSource.Play();
					startMusic = false;
				}

				clearSpawnedObjects();

				if (selectingMap) {
					scoreboardCanvas.SetActive(false);

					if (mapSlectionUI.done) {
						mapSlectionUI.done = false;
						mapSlectionUI.gameObject.SetActive (false);
						scoreboardCanvas.SetActive(true);
						selectingMap = false;
					}
					return;
				}

				string timeText;
				timeText = (int)((timer + 1) / 60) + ":" + (int)(((timer + 1) % 60) / 10) + (int)(((timer + 1) % 60) % 10);
				scoreboard.updateScoreboardMessage(timeText);

				creator.money = mapinfo.mapMoney;
				creator.ui.updateMoneyText(mapinfo.mapMoney);

				if (timer <= 0)
				{
					timer = 90f;
					scoreboardCanvas.gameObject.SetActive(false);
					creatorContainer.gameObject.SetActive(true);

					if (!mapContainer)
						generateMap();

					enablePowerUps();

					// Position creator at start
					Vector3 tempPos = mapinfo.startLocation.transform.position;
					tempPos.z = creator.transform.position.z;
					creator.transform.position = tempPos;

					if (currCreator == 0) {
						creatorRenderer.color = new Color(player1Color.r, player1Color.g, player1Color.b);
					} else {
						creatorRenderer.color = new Color(player2Color.r, player2Color.g, player2Color.b);
					}

					camera.setFollowing(creator.gameObject);
					nextState();
					//set bool for beginning of next round
					startMusic = true;
				}
				break;
			}
		case 4: // END GAME
			{
				if (scores [0] < scores [1]) {
					scoreboard.setLoser (0);
					winScore = scores [1];
				} else if (scores [1] < scores [0]) {
					scoreboard.setLoser (1);
					winScore = scores [0];
				}
				else
					scoreboard.setLoser (3);

				// Look for possible new high score:
				if (Input.GetButtonDown ("A_1") || Input.GetButtonDown ("A_2")) {
					SceneManager.LoadScene ("InputName");
				}
				break;
			}
		}

		timer -= Time.deltaTime;
	} 
	//End of update Method

	private void nextState()
	{
		//set state of next round
		if (state >= 3)
			state = 0;
		else
			state = state + 1;
	}

	private void createPlayer()
	{
		playerContainer = Instantiate(playerPrefab).transform;
		player = playerContainer.Find("PlayerEnt").GetComponent<PlayerController>();
		playerUI = playerContainer.Find("PlayerUI").GetComponent<PlayerHud>();
		playerSprites = player.gameObject.GetComponentsInChildren<SpriteRenderer>();
		camera.setFollowing(player.gameObject);

		if (twoControllers)
		{
			player.setController(1);
		}
		else
		{
			player.setController(1);
		}
	}

	private void createCreator()
	{
		creatorContainer = Instantiate(creatorPrefab).transform;
		creator = creatorContainer.Find("CreatorEnt").GetComponent<CreatorController>();
		creatorUI = creatorContainer.Find("CreatorUI").GetComponent<CreatorHud>();
		creatorRenderer = creator.gameObject.GetComponent<SpriteRenderer>();
		camera.setFollowing(creator.gameObject);

		if (twoControllers)
		{
			creator.setController(2);
		}
		else
		{
			creator.setController(1);
		}
	}

	private void clearSpawnedObjects()
	{
		foreach (Transform spawned in spawnedContainer.transform)
			Destroy(spawned.gameObject);
	}

	private void enablePowerUps()
	{
		foreach (Transform powerUp in mapContainer.transform.Find("PowerUps"))
			powerUp.gameObject.SetActive(true);
	}

	public void respawnPlayer()
	{
		player.startingHealth += 20f;
		player.resetEverything ();
		playerUI.powerUpTimer = 0f;

		// Position player at start
		Vector3 tempPos = mapinfo.startLocation.transform.position;
		tempPos.z = player.transform.position.z;
		player.transform.position = tempPos;

		/*
		GameObject.Destroy (player.gameObject);
		Transform newPlayerEnt = ((GameObject)Instantiate (playerEntPrefab, playerContainer)).transform;
		player = newPlayerEnt.GetComponent<PlayerController> ();
		playerSprites = player.gameObject.GetComponentsInChildren<SpriteRenderer>();
		camera.setFollowing(player.gameObject);
		*/

		scores[currPlayer] -= 100;
		scores[currCreator] += 100;
	}

	public void applyGameObject(GameObject child)
	{
		child.transform.SetParent(spawnedContainer.transform);
	}

	public void generateMap()
	{
		GameObject mapGo;
		if (randomMaps) {
			Object[] mapOs = Resources.LoadAll ("Maps/");
			int rnd = Random.Range (0, mapOs.Length);
			mapGo = (GameObject)mapOs [rnd];
		} else {
			string mapPath = "Maps/" + SettingsManager.mapToLoad;
			mapGo = Resources.Load(mapPath, typeof(GameObject)) as GameObject;
		}

		if (mapGo.GetComponent<MapInfo> () && mapGo.name.ToCharArray () [0] != '~') {
			mapContainer = Instantiate (mapGo);
			mapinfo = mapContainer.GetComponent<MapInfo> ();

			// Read the level colors
			camera.GetComponent<Camera> ().backgroundColor = mapinfo.backColor;
			camera.transform.Find ("Back").GetComponentInChildren<ParticleSystem> ().startColor = mapinfo.particleColor1;
			camera.transform.Find ("Back2").GetComponentInChildren<ParticleSystem> ().startColor = mapinfo.particleColor2;
		} else {
			// Try again... ignore how poor practice this is
			generateMap();
		}
	}

	public void endPlayerPhase()
	{
		playerReachedEnd = true;
	}
}
