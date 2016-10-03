using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using Lean;
using LitJson;

public enum Tiles {
	// TODO: Update these...
	Unset,
	Ground,
	Spike,
	Water,
	Rainbow
}

[Serializable]
public class TileSprite {
	public string tName;
	public Sprite tImage;
	public Tiles tType;

	public TileSprite() {
		tName = "Unset";
		tImage = new Sprite ();
		tType = Tiles.Unset;
	}

	public TileSprite(string name, Sprite image, Tiles tile)
	{
		this.tName = name;
		this.tImage = image;
		this.tType = tile;
	}
}

public class TileMapController : MonoBehaviour {

	public List<TileSprite> tileSprites;
	public Vector2 mapSize;
	public Sprite defaultImage;
	public GameObject tileContainerPrefab;
	public GameObject tilePrefab;
	// public GameObject toFollow;

	private Vector2 currentPosition;
	private TileSprite[,] _map;
	private GameObject controller;
	private GameObject _tileContainer;
	private List<GameObject> _tiles = new List<GameObject>();

	private string jsonString;
	private JsonData objJson;

	private JsonData generateJsonObject (string fname)
	{
		jsonString = File.ReadAllText(Application.dataPath + "/JSON_files/" + fname);
		Debug.Log (jsonString);
		objJson = JsonMapper.ToObject (jsonString);

		return objJson;
	}

	private TileSprite findTile(Tiles tile)
	{
		foreach (TileSprite tileSprite in tileSprites) {
			if (tileSprite.tType == tile)
				return tileSprite;
		}
		return null;
	}

	// Used to create default tiles
	// Not used currently cause I only want tiles that the map
	//	wants has specified to spawn
	private void defaultTiles() 
	{
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				_map [x, y] = new TileSprite ("Unset", defaultImage, Tiles.Unset);
			}
		}
	}

	private void generateTiles(int x, int y, int tID)
	{
		TileSprite tSprite = new TileSprite (tileSprites[tID].tName, 
											 tileSprites[tID].tImage, 
											 tileSprites[tID].tType);
		_map [x, (int)mapSize.y - y] = tSprite;
	}

	private void setTiles(JsonData map)
	{
		int index = 0;
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				int tileID = (int) map ["layers"] [0] ["data"] [index];
				if (tileID != 0)
					generateTiles (x, y, tileID);
				index++;
			}
		}
	}

	private void addTilesToWorld()
	{
		foreach (GameObject o in _tiles) {
			LeanPool.Despawn (o);
		}
		_tiles.Clear ();
		LeanPool.Despawn(_tileContainer);
		_tileContainer = LeanPool.Spawn (tileContainerPrefab);

		// Offset the map so that the map is generated and centered on the map
		//  at (0, 0)
		int offSetX = (int) mapSize.x / 2;
		int offSetY = (int) mapSize.y / 2;

		for (float y = 0; y < mapSize.y; y++) {
			for (float x = 0; x < mapSize.x; x++) {
				if (_map [(int)x, (int)y] == null)
					continue;
				GameObject t = LeanPool.Spawn (tilePrefab);
				t.transform.position = new Vector3 (x - offSetX, y - offSetY, -8);
				t.transform.SetParent (_tileContainer.transform);
				SpriteRenderer renderer = t.GetComponent<SpriteRenderer> ();
				renderer.sprite = _map [(int)x , (int)y].tImage;
				_tiles.Add (t);
			}
		}
	}

	// Checks to see if the current tile in the tilemap is an edge tile
	private bool isEdgeTile(int x, int y)
	{
		if (_map [x, y] == null)
			return false;
		/*
		0 0 0 0 0 0 0 0
		0 1 1 1 1 1 1 0
		0 1 1 0 0 1 1 0
		1 1 1 0 0 1 1 1 
		*/
		// PolygonCollider2D setPath(index, point = vector2) pathCount()

		int startX 	= (x - 1 < 0) ? x : x - 1;
		int startY 	= (y - 1 < 0) ? y : y - 1;
		int endX 	= (x + 1 > mapSize.x - 1) ? x : x + 1;
		int endY 	= (y + 1 > mapSize.y - 1) ? y : y + 1;

		for (int cRow = startX; cRow <= endX; cRow++)
		{
			for (int cCol = startY; cCol <= endY; cCol++)
			{
				if ( (_map[cRow, cCol] == null) && !(cRow == x && cCol == y))
					return true;
			}
		}
		return false;
	}

	private void createCollider() 
	{
		int[,] edgeTiles = new int[(int) mapSize.x, (int) mapSize.y];
		string toPrint = "";
		for (int y = 0; y < mapSize.y; y++) {
			for (int x = 0; x < mapSize.x; x++) {
				if (isEdgeTile (x, y)) {
					toPrint += "1";
					// while loopy loop
					/*
					Collider col = LeanPool.Spawn(collider);
					col.transform.SetParent (_colliderContainer.transform);
					_colliders.Add(col);
					*/
				} else {
					toPrint += "0";
				}
			}
			toPrint += "\n";
		}
		print (toPrint);
	}

	public void Start()
	{
		// controller = GameObject.Find ("TileMapController");
		// Camera cam = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		// float height = 2f * cam.orthographicSize;
		// float width = height * cam.aspect;

		JsonData map = generateJsonObject ("hello_hgd.json");
		mapSize.x = (float) map["width"];
		mapSize.y = (float) map["height"];

		_map = new TileSprite[(int) mapSize.x, (int) mapSize.y];

		setTiles (map);
		addTilesToWorld ();
	}

	private void Update()
	{}
}