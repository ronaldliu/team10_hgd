using UnityEngine;
using UnityEditor;
using System.Collections;

public class MapEditor : EditorWindow {

	bool mapInScene;
	GameObject sawObj;
	Texture saw; //TODO test

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

	void Awake() {
		mapInScene = false;
		saw = (Resources.Load ("Traps/Saw") as GameObject).GetComponent<SpriteRenderer> ().sprite.texture;
		mapName = "Map";
	}

	void OnGUI() {
		
		GUILayout.Label ("Map Settings", EditorStyles.boldLabel);
		mapName = EditorGUILayout.TextField ("Map Name", mapName);

		sawObj = (GameObject)EditorGUILayout.ObjectField ("Saw", sawObj, typeof(GameObject), false);

		if (!mapInScene) {

			if (GUILayout.Button ("Generate Map \"" + mapName + "\"")) {
				mapInScene = createMap ();
			}
		}
		else {

			mapInfo.timeToFinish = EditorGUILayout.FloatField ("Time to Finish (sec)", mapInfo.timeToFinish);
			mapInfo.mapMoney = EditorGUILayout.IntField ("Map Money", mapInfo.mapMoney);

			EditorGUILayout.BeginHorizontal ();
			float boxSize = (Screen.width - GUI.skin.window.padding.horizontal) / 2;
			GUILayout.Box (saw, GUILayout.Width(boxSize), GUILayout.Height(boxSize));
			GUILayout.Box (saw, GUILayout.Width(boxSize), GUILayout.Height(boxSize));
			EditorGUILayout.EndHorizontal ();

			handleDrag ();
		}
	}

	void handleDrag() {
		switch (Event.current.type) {
		case EventType.MouseDrag:
			DragAndDrop.PrepareStartDrag ();
			Object[] ar = { sawObj };
			DragAndDrop.objectReferences = ar;
			DragAndDrop.StartDrag ("BOOP");

			Event.current.Use ();
			break;
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
		mapInfo = mapObj.GetComponent<MapInfo> ();

		return true;
	}

	//Pull MapInfo from mapObj
	void readInfo() {
		mapName = mapObj.name;
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}

	//Push MapInfo to mapObj
	void WriteInfo() {
		mapObj.name = mapName;
		mapInfo = mapObj.GetComponent<MapInfo> ();

	}
}
