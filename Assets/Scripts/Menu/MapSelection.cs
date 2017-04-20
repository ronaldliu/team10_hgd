using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MapSelection : MonoBehaviour {

	GameObject canvas;
	public Image image;
	Text mapText;
	GameObject knob;
	float knobWidth;

	int selected;
	bool canInteract;

	List<MapInfo> maps;
	List<GameObject> knobs;

	public bool done;
	public Camera previewCam;

	GameObject back;
	GameObject back2;

	// Use this for initialization
	void Start () {
		canvas = GameObject.Find ("MapSelectionCanvas");
		image = GameObject.Find ("PreviewImage").GetComponent<Image>();
		mapText = GameObject.Find ("MapText").GetComponent<Text>();
		knob = GameObject.Find ("Knob");
		knobWidth = knob.GetComponent<RectTransform> ().rect.width;

		selected = 0;
		canInteract = true;

		//Load all the maps
		maps = new List<MapInfo>();
		Object[] mapOs = Resources.LoadAll ("Maps/");
		foreach(Object o in mapOs) {
			GameObject mapGo = (GameObject)o;
			if (mapGo.GetComponent<MapInfo> () && mapGo.name.ToCharArray () [0] != '~') {
				maps.Add (mapGo.GetComponent<MapInfo> ());
			}
		}

		//Create the knobs at the bottom
		knobs = new List<GameObject>();
		for (int i = 0; i < maps.Count; i++) {
			GameObject newKnob = (GameObject)GameObject.Instantiate (knob, canvas.transform);
			float newX = (i * knobWidth);
			newKnob.transform.localPosition = new Vector3 (newX, newKnob.transform.localPosition.y, newKnob.transform.localPosition.z);
			newKnob.GetComponent<RawImage> ().color = maps [i].backColor;
			if (i == 0) {
				newKnob.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
			}
			knobs.Add (newKnob);
		}

		done = false;
		if (GameObject.Find ("Game")) {
			back = GameObject.Find ("Back");
			back2 = GameObject.Find ("Back2");
			GameObject.Find ("Prompt_B").SetActive(false);
		}
		takeSnapshotOfMap ();
	}
	
	// Update is called once per frame
	void Update () {
		float xOne = Input.GetAxisRaw ("L_XAxis_1");
		float xTwo = Input.GetAxisRaw ("L_XAxis_2");
		if ((Mathf.Abs(xOne) > 0.1f || Mathf.Abs(xTwo) > 0.1f) && canInteract) {
			canInteract = false;
			StartCoroutine (SelectionChange (xOne, xTwo));
		}
		if (Input.GetButtonDown ("A_1") || Input.GetButtonDown ("A_2")) {
			GameObject.Find ("SettingsHolder").GetComponent<MatchSettingsHolder> ().mapToLoad = maps [selected].name;
			if(GameObject.Find("Game"))
				done = true;
			else
				SceneManager.LoadScene ("FinalGame");
		}
		if (Input.GetButtonDown ("B_1") || Input.GetButtonDown ("B_2")) {
			if(!GameObject.Find("Game"))
				SceneManager.LoadScene ("MatchSettings");
		}

		foreach (GameObject go in knobs) {
			float alpha = Mathf.Abs (go.transform.localPosition.x) > knobWidth * 2? 0f : 1f;
			Color color = go.GetComponent<RawImage> ().color;
			go.GetComponent<RawImage> ().color = new Color(color.r, color.g, color.b, alpha);
		}
	}

	IEnumerator SelectionChange(float xOne, float xTwo)
	{
		if (xOne > 0.1f || xTwo > 0.1f) {
			//move right
			if (selected < maps.Count - 1) {
				selected++;
				mapText.text = "< " + maps [selected].name + " >";
				takeSnapshotOfMap ();

				foreach (GameObject go in knobs) {
					Vector3 pos = go.transform.localPosition;
					go.transform.localPosition = new Vector3 (pos.x - knobWidth, pos.y, pos.z);

					if (go.transform.localPosition.x == 0) {
						go.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
					} else {
						go.transform.localScale = new Vector3 (1, 1, 1);
					}
				}
			}
		} else if (xOne < -0.1f || xTwo < -0.1f) {
			//move left
			if (selected > 0) {
				selected--;
				mapText.text = "< "  + maps [selected].name + " >";
				takeSnapshotOfMap ();

				foreach (GameObject go in knobs) {
					Vector3 pos = go.transform.localPosition;
					go.transform.localPosition = new Vector3 (pos.x + knobWidth, pos.y, pos.z);

					if (go.transform.localPosition.x == 0) {
						go.transform.localScale = new Vector3 (1.5f, 1.5f, 1);
					} else {
						go.transform.localScale = new Vector3 (1, 1, 1);
					}
				}
			}
		}
		yield return new WaitForSeconds (0.2f);
		canInteract = true;
	}

	Texture2D screenShot;
	Sprite sprite;

	private void takeSnapshotOfMap() {

		if (back) {
			back.SetActive (false);
			back2.SetActive (false);
		}

		Camera cam = Instantiate (previewCam.gameObject).GetComponent<Camera>();
		MapInfo map = maps [selected];
		GameObject mapOb = Instantiate (map.gameObject);

		float mostLeft = 0;
		float mostRight = 0;
		float mostUp = 0;
		float mostDown = 0;

		foreach (BoxCollider2D box in mapOb.GetComponentsInChildren<BoxCollider2D>()) {
			if (box.GetComponent<PlatformController> ()) {
				Bounds b = box.bounds;
				if (b.center.x - b.extents.x < mostLeft)
					mostLeft = b.center.x - b.extents.x;
				if (b.center.x + b.extents.x > mostRight)
					mostRight = b.center.x + b.extents.x;
				if (b.center.y + b.extents.y > mostUp)
					mostUp = b.center.y + b.extents.y;
				if (b.center.y - b.extents.y < mostDown)
					mostDown = b.center.y - b.extents.y;
			}
		}

		int resWidth = (int)image.rectTransform.rect.width;
		int resHeight = (int)image.rectTransform.rect.height;

		//Position camera
		float aspectRatio = ((float)resWidth / (float)resHeight);
		cam.orthographicSize = (Mathf.Max(mostUp - mostDown + 5, (mostRight - mostLeft) / aspectRatio + 5) / 2);
		cam.transform.position = new Vector3 ((mostLeft + mostRight) / 2, ((mostUp + mostDown) / 2), -10);
		cam.backgroundColor = map.backColor;

		RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
		cam.targetTexture = rt;
		screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
		cam.Render();
		RenderTexture.active = rt;
		screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
		screenShot.Apply ();
		sprite = Sprite.Create(screenShot, new Rect(0f, 0f, resWidth, resHeight), new Vector2(0.5f, 0.5f), 100f);
		image.sprite = sprite;
		cam.targetTexture = null;
		RenderTexture.active = null;
		Destroy(rt);
		Destroy (mapOb);
		Destroy (cam.gameObject);

		if (back) {
			back.SetActive (true);
			back2.SetActive (true);
		}
	}
}
