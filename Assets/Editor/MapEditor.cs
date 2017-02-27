using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow {

	bool mapInScene;
	Transform platforms, startLocation, endFlag, outOfBounds, powerUps;
	List<GameObject> placablePowerUps;
	List<GameObject> placableWeapons;
	GameObject mousedOver;

	static GameObject draggedObj;

	//Texture saw; //TODO test

	public static bool meme;

	//Map references
	GameObject mapObj;
	MapInfo mapInfo;

	//Map Info
	string mapName;

	//Add this window to the Window menu
	[MenuItem ("Window/MapEditor")]
	public static void ShowWindow() {
		EditorWindow.GetWindow (typeof(MapEditor));
	}

	void OnFocus() {
		// Remove delegate listener if it has previously
		// been assigned.
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
		SceneView.onSceneGUIDelegate += OnSceneGUI;

		if (!mapInScene) {
			if (mapObj = GameObject.FindObjectOfType<MapInfo> ().gameObject) {
				readInfo ();
				mapInScene = true;
			}
		}

		if (placablePowerUps == null) {
			placablePowerUps = new List<GameObject> ();
			placableWeapons = new List<GameObject> ();
			Object[] placables = Resources.LoadAll ("PowerUps/");
			foreach (Object obj in placables) {
				if (((GameObject)obj).GetComponent<PickUpPower> () != null)
					placablePowerUps.Add ((GameObject)obj);
				if (((GameObject)obj).GetComponent<PickUpWeapon> () != null)
					placableWeapons.Add ((GameObject)obj);
			}
		}
	}

	void OnDestroy() {
		// When the window is destroyed, remove the delegate
		// so that it will no longer do any drawing.
		SceneView.onSceneGUIDelegate -= OnSceneGUI;
	}

	void Awake() {
		mapInScene = false;
		meme = false;
		mapName = "Map";
	}

	private static void OnSceneGUI(SceneView sceneview)
	{
		Handles.BeginGUI();

		//if (GUILayout.Button("Press Me"))
		//	Debug.Log("Got it to work.");

		if(meme)
			GUI.Label (new Rect(Screen.width - 250, Screen.height - 250, 250, 250), Resources.Load("meme") as Texture);


		if (Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform)
		{
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy; // show a drag-add icon on the mouse cursor

			if (draggedObj == null)
				draggedObj = (GameObject)Object.Instantiate(DragAndDrop.objectReferences[0]);

			// compute mouse position on the world y=0 plane
			Ray mouseRay = Camera.current.ScreenPointToRay(new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y, 0.0f));
			if (mouseRay.direction.y < 0.0f)
			{
				float t = -mouseRay.origin.y / mouseRay.direction.y;
				Vector3 mouseWorldPos = mouseRay.origin + t * mouseRay.direction;
				mouseWorldPos.y = 0.0f;

				draggedObj.transform.position = mouseWorldPos;
			}

			if (Event.current.type == EventType.DragPerform)
			{
				DragAndDrop.AcceptDrag();
				draggedObj = null;
			}

			Event.current.Use();
		}

		/*
		switch (Event.current.type) {
		case EventType.DragPerform:
			DragAndDrop.AcceptDrag ();
			//
			DragAndDrop.PrepareStartDrag ();
			Object[] or = { mousedOver };
			DragAndDrop.objectReferences = or;
			DragAndDrop.StartDrag ("BOOP");

			Event.current.Use ();
			break;
		} */

		Handles.EndGUI();
	}

	void OnGUI() {
		
		GUILayout.Label ("Map Settings", EditorStyles.boldLabel);
		mapName = EditorGUILayout.TextField ("Map Name", mapName);

		if (!mapInScene) {

			if (GUILayout.Button ("Generate Map \"" + mapName + "\"")) {
				mapInScene = createMap ();
			}
		}
		else {

			mapInfo.timeToFinish = EditorGUILayout.FloatField ("Time to Finish (sec)", mapInfo.timeToFinish);
			mapInfo.mapMoney = EditorGUILayout.IntField ("Map Money", mapInfo.mapMoney);

			mousedOver = null;

			GUILayout.Label ("PowerUps", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal ();
			float boxSize = (Screen.width / placablePowerUps.Count) - GUI.skin.box.margin.horizontal;
			foreach (GameObject go in placablePowerUps) {
				Texture goTex;
				if (goTex = go.GetComponent<SpriteRenderer> ().sprite.texture) {
					GUILayout.Box (goTex, GUILayout.Width(boxSize), GUILayout.Height(boxSize));

					//Check if this box was moused over
					if (GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition)) {
						mousedOver = go;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			GUILayout.Label ("Weapons", EditorStyles.boldLabel);

			EditorGUILayout.BeginHorizontal ();
			boxSize = (Screen.width / placableWeapons.Count) - GUI.skin.box.margin.horizontal;
			foreach (GameObject go in placableWeapons) {
				Texture goTex;
				if (goTex = go.GetComponent<PickUpWeapon> ()
						.attachedWeaponPrefab.GetComponent<SpriteRenderer> ().sprite.texture) {
					GUILayout.Box (goTex, GUILayout.Width(boxSize), GUILayout.Height(boxSize));

					//Check if this box was moused over
					if (GUILayoutUtility.GetLastRect ().Contains (Event.current.mousePosition)) {
						mousedOver = go;
					}
				}
			}
			EditorGUILayout.EndHorizontal ();

			meme = EditorGUILayout.Toggle ("Meme", meme);

			handleDrag ();
		}
	}

	void handleDrag() {
		if (mousedOver) {
			switch (Event.current.type) {
			case EventType.MouseDrag:
				DragAndDrop.PrepareStartDrag ();
				Object[] or = { mousedOver };
				DragAndDrop.objectReferences = or;
				DragAndDrop.StartDrag ("BOOP");

				Event.current.Use ();
				break;
			}
		}
	}

	//Returns whether the map was created or not
	bool createMap() {
		//Check if this map name exists already
		if (Resources.Load ("Maps/" + mapName) as GameObject) {
			bool result = EditorUtility.DisplayDialog ("Map Already Exists", "A map with this name already exists," +
				" would you like to load it?", "OK", "Never Mind");
			if (!result)
				return false;
			else {
				//TODO
				//Load existing map
				return false;
			}
		}

		//Generate an empty map using the template
		GameObject template = Resources.Load ("Maps/Map_Template") as GameObject;

		mapObj = Instantiate (template);
		mapObj.name = mapName;

		platforms = mapObj.transform.Find ("Platforms");
		startLocation = mapObj.transform.Find ("StartLocation");
		endFlag = mapObj.transform.Find ("EndFlag");
		outOfBounds = mapObj.transform.Find ("OutOfBounds");
		powerUps = mapObj.transform.Find ("PowerUps");
		mapInfo = mapObj.GetComponent<MapInfo> ();

		return true;
	}

	//Pull MapInfo from mapObj
	void readInfo() {
		mapName = mapObj.name;
		platforms = mapObj.transform.Find ("Platforms");
		startLocation = mapObj.transform.Find ("StartLocation");
		endFlag = mapObj.transform.Find ("EndFlag");
		outOfBounds = mapObj.transform.Find ("OutOfBounds");
		powerUps = mapObj.transform.Find ("PowerUps");
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}

	//Push MapInfo to mapObj
	void WriteInfo() {
		mapObj.name = mapName;
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}
}
