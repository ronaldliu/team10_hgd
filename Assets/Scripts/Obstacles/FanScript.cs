using UnityEngine;
using System.Collections;

public class FanScript : MonoBehaviour {

	public int numberOfPoints = 8; //number of raycasts to do to find points
	public int effectDistance = 30; //max distance a raycast can travel
	private int ppu = 32;

	// Use this for initialization
	void Start () {
		Debug.Log ("Doing collision raycast calculation");
		calculateAndUpdateCollider ();
	}

	void calculateAndUpdateCollider() {
		int width = this.gameObject.GetComponent<SpriteRenderer> ().sprite.texture.width;
		int height = this.gameObject.GetComponent<SpriteRenderer> ().sprite.texture.height;
		int halfW = width / 2; //warning loss of percision
		int halfH = height / 2; //warning loss of percision
		float halfWA = (float)halfW / (float)ppu;
		float heightA = (float)height / (float)ppu;
		//Vector2[] points = new Vector2[this.numberOfPoints + 4];
		Vector2[] points = new Vector2[4];
		points [0] = new Vector2 (halfWA, heightA);
		points [1] = new Vector2 (halfWA, 0);
		points [2] = new Vector2 (-halfWA, 0);
		points [3] = new Vector2 (-halfWA, heightA);
		double distanceOffset = (double)width / (double)this.numberOfPoints;
		Quaternion rotation = transform.localRotation;
		PolygonCollider2D coll = this.GetComponent<PolygonCollider2D> ();
		for (int i = 0; i < this.numberOfPoints; i++) {
			RaycastHit2D hit = Physics2D.Raycast (new Vector2(0,0) /* fix dis */, rotation * Vector2.up, 20);
		}
		coll.SetPath (0, points);

	}

	// Update is called once per frame
	void Update () {
	
	}
}
