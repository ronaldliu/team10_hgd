using UnityEngine;
using System.Collections;

public class CreatorController : MonoBehaviour {

	public GameObject[] availableObjs;
	public float moveSpeed;
	// Used to get reference to see who is the current player
	private GameController game;
	private int currObj;
	private Transform currObjRenderer;
	public Transform snappedEdge;
	public Vector3 snappedEdgePos;
	public int snappedEdgeSide;

	public CreatorHud ui;

	// Use this for initialization
	void Start () {
		// game = GameObject.Find ("UI").GetComponent<GameController> ();
		// print (game.player);
		currObj = 0;
		currObjRenderer = transform.Find ("currentObj");
		setObjRenderer ();
	}
	
	// Update is called once per frame
	void Update () {
		float inputXAmount = Input.GetAxis ("L_XAxis_1");
		float inputYAmount = Input.GetAxis ("L_YAxis_1");

		if (Input.GetButtonDown ("A_1"))
			spawnGameObject ();

		if (Input.GetButtonDown ("RB_1")) {
			if (currObj < availableObjs.Length - 1)
				currObj++;
			else
				currObj = 0;
			setObjRenderer ();
		}
		if (Input.GetButtonDown ("LB_1")) {
			if (currObj > 0)
				currObj--;
			else
				currObj = availableObjs.Length - 1;
			setObjRenderer ();
		}
		// Calculate how much the velocity should change based on xAccel
		Vector3 direction = new Vector3 (inputXAmount, -inputYAmount, 0.0f);
		transform.Translate (moveSpeed * direction * Time.deltaTime);

		GetComponent<CircleCollider2D> ().attachedRigidbody.WakeUp();

		// Reset the snapped object if out of range of any cubes
		LayerMask platforms = LayerMask.GetMask("Platforms");
		if (!GetComponent<CircleCollider2D> ().IsTouchingLayers (platforms) || Input.GetButton ("Y_1")) {
			// Reset the object snapped to
			snappedEdge = null;
			currObjRenderer.localPosition = new Vector3 (0, 0, 0);
			currObjRenderer.eulerAngles = new Vector3 (0, 0, 0);
			snappedEdgeSide = 0;
		} else {
			/* Update currObjectRenderer's position/rotation on the snapped edge
			 * manually, as this may not be called every frame. This prevents the
			 * object from being shaky when moving. */
			if(snappedEdge != null)
				OnTriggerStay2D (snappedEdge.gameObject.GetComponent<BoxCollider2D> ());
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		// Don't snap to the edge if Y is held (debug)
		// TODO Remove this when needed
		if (Input.GetButton ("Y_1"))
			return;

		// Get the object that was collided with
		GameObject obj = other.gameObject;

		// Forget this object if it is not a platform object
		if (obj.layer != LayerMask.NameToLayer("Platforms"))
			return;

		// Find the closest point within the platform's bounds
		Vector3 objFacing = obj.transform.rotation * Vector3.up;

		Vector3 closestPos = Vector3.zero;
		Vector3 dir = objFacing;
		for (int i = 0; i < 4; i++) {
			RaycastHit2D hit = Physics2D.Raycast (transform.position, dir, 2, LayerMask.GetMask ("Platforms"));
			if (hit.collider != null) {
				closestPos = hit.point;
				break;
			}
			dir = Quaternion.Euler (0f, 0f, 90f) * dir;
		}

		//TODO Fix maybe?
		if (closestPos == Vector3.zero)
			return;

		// Calculate that point relative to the Creator
		Vector3 relativePos = closestPos - transform.position;
		float distance = relativePos.magnitude;
		// Forget this object if there is a closer platform than this one
		if (snappedEdge != null && snappedEdge != obj.transform && distance > snappedEdgePos.magnitude)
			return;

		// Update snappedEdge
		snappedEdge = obj.transform;
		snappedEdgePos = relativePos;

		// Update rotation
		Vector3 dirFace = Quaternion.Euler (0f, 0f, 90f) * dir;
		float rotationAngle = Mathf.Atan2 (dirFace.y, dirFace.x) * Mathf.Rad2Deg;
		currObjRenderer.eulerAngles = new Vector3(0, 0, rotationAngle);

		// Set currObjectRenderer's position to the edge's
		currObjRenderer.localPosition = snappedEdgePos;
	}

	private void spawnGameObject()
	{
		GameObject spawned = Instantiate (availableObjs [currObj]);
		spawned.transform.position = currObjRenderer.position;
		spawned.transform.rotation = currObjRenderer.rotation;
		spawned.GetComponent<SentryController>().enabled = false;
		Debug.Log ("Creator has created: " + spawned.name);
	}

	private void setObjRenderer()
	{
		SpriteRenderer currSelectedSprite = availableObjs [currObj].GetComponent<SpriteRenderer> ();
		currObjRenderer.GetComponent<SpriteRenderer>().sprite = currSelectedSprite.sprite;
		Color temp = currObjRenderer.GetComponent<SpriteRenderer> ().color;
		temp.a = 0.7f;
		currObjRenderer.GetComponent<SpriteRenderer> ().color = temp;
	}
}
