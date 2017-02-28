using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class NameInput : MonoBehaviour {
	
	public Text please;
	public Text letter1;
	public Text letter2;
	public Text letter3;
	public Text cont;
	private Text[] letterText = new Text[3];

	private string[] letters = {
		"A",
		"B",
		"C",
		"D",
		"E",
		"F",
		"G",
		"H",
		"I",
		"J",
		"K",
		"L",
		"M",
		"N",
		"O",
		"P",
		"Q",
		"R",
		"S",
		"T",
		"U",
		"V",
		"W",
		"X",
		"Y",
		"Z"
	};
	private string hsName = "";

	private int[] index = { 0, 0, 0 };
	private int pos = 0;

	private bool tapped = false;

	private float lastTap = 0f;
	private float wait = 0.3f;

	void Start() {

		// Debug!!!
		//PlayerPrefs.DeleteAll ();

		// Add winner's score to check if it's in top 10, go to main menu if not:
		int x = Leaderboard.setHS(GameController.winScore, "temp");
		if (x == 0) {
			SceneManager.LoadScene ("MainMenu");
		}

		letterText [0] = letter1;
		letterText [1] = letter2;
		letterText [2] = letter3;
		
		letterText[0].text = letters [index[0]];
		letterText[1].text = letters [index[1]];
		letterText[2].text = letters [index[2]];
		letterText[0].color = Color.green;

		please.text = "New High Score\nPlease Enter Your Name:";
	}

	void Update() {

		letterText[0].text = letters [index[0]];
		letterText[1].text = letters [index[1]];
		letterText[2].text = letters [index[2]];

		if (Time.frameCount % 128 > 0 && Time.frameCount % 128 < 64 && hsName.Equals("")) {
			cont.text = "Press Start To Continue";
		}
		else
			cont.text = "";

		// Tapping up on joystick, moves up alphabet:
		if (Input.GetAxis("L_YAxis_1") < 0 && !tapped) {
			lastTap = Time.time;
			tapped = true;
			if (index [pos] == 25) {
				index [pos] = 0;
			} else {
				index [pos]++;
			}
		} 
		else {
			if (Time.time - lastTap > wait)
				tapped = false;
		}

		// Tapping down on joystick, moves down alphabet:
		if (Input.GetAxis("L_YAxis_1") > 0 && !tapped) {
			lastTap = Time.time;
			tapped = true;
			if (index [pos] == 0) {
				index [pos] = 25;
			} else {
				index [pos]--;
			}
		}
		else {
			if (Time.time - lastTap > wait)
				tapped = false;
		}

		// Moving to left on joystick, switches to different letter:
		if (Input.GetAxis("L_XAxis_1") < 0 && !tapped) {
			lastTap = Time.time;
			tapped = true;
			if (pos == 0) {
				pos = 2;
				letterText [0].color = Color.red;
				letterText [2].color = Color.green;
			} else {
				pos--;
				letterText [pos + 1].color = Color.red;
				letterText [pos].color = Color.green;
			}
		}
		else {
			if (Time.time - lastTap > wait)
				tapped = false;
		}

		// Moving to right on joystick, switches to different letter:
		if (Input.GetAxis("L_XAxis_1") > 0 && !tapped){
			lastTap = Time.time;
			tapped = true;
			if (pos == 2) {
				pos = 0;
				letterText [2].color = Color.red;
				letterText [0].color = Color.green;
			} else {
				pos++;
				letterText [pos - 1].color = Color.red;
				letterText [pos].color = Color.green;
			}
		}
		else {
			if (Time.time - lastTap > wait)
				tapped = false;
		}
			
		if (Input.GetButtonDown("Start_1")) {

			// Store player name:
			hsName = letter1.text + letter2.text + letter3.text;

			// Load high scores until new score found:
			int ctr = 0;
			var hs = Leaderboard.lbScores [ctr];
			while (!hs.name.Equals ("temp")) {
				ctr++;
				hs = Leaderboard.lbScores [ctr];
			}

			// Change name, save it, go to leaderboard:
			hs.name = hsName;
			PlayerPrefs.SetString ("x[" + ctr + "].name", hs.name);
			SceneManager.LoadScene ("Leaderboard");

		}
	}
}
