using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CreatorHud : MonoBehaviour {

	public Text timerText;
	public Text roundText;

	private CreatorController creator;
	private float timer;

	// Use this for initialization
	void Start () {
		creator = GameObject.Find ("CreatorUI").GetComponent<CreatorController> ();
		eraseAllText ();
	}

	public void updateTimers(string timeFromGame) {
		timerText.text = timeFromGame;
	}

	// PRIVATE FUNCTIONS
	private void eraseAllText()
	{
		timerText.text = "";
		roundText.text = "";
	}
}
