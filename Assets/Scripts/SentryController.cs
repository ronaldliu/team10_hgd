using UnityEngine;
using System.Collections;

public class SentryController : WeaponController {

	public Transform Target;
	public float RotateSpeed;
	private Quaternion lookRotation;
	private Vector3 direction;
	public GameObject bulletTrailPrefab;
	public float health;
	int i = 10;

	public override void Fire() {
		if (Time.time > nextFire) {
			// For calculating Variance in shooting to emulate bullet spread
			// Multiply by 0.01 so that the values can be changed easily
			shootDir = firePoint.right;
			shootDir.y += Random.Range (-1 * variance * 0.01F, variance * 0.01F);

			nextFire = Time.time + 1 / fireRate;
			if (!shotObject)
				shootRay ();
			else
				shootProjectile ();{
			}
		}
	}

	void shootRay () {
		// Ray2D rShot = new Ray2D (firePoint.position, firePoint.right * 100);
		Debug.DrawRay (firePoint.position, shootDir * 100, Color.green);

		RaycastHit2D hit = Physics2D.Raycast (firePoint.position, shootDir, 100, canBeShot);
		if (hit) {
			generateTrail (hit.distance);
			print ("Hit: " + hit.transform.name);
			if (LayerMask.LayerToName (hit.transform.gameObject.layer) == "Platforms") {
				// Do things to the enemy
			} else if (LayerMask.LayerToName (hit.transform.gameObject.layer) == "Enemies") {

			}
		} else {
			generateTrail ();
		}
	}

	void generateTrail(float distance = -1F)
	{
		float angle = Mathf.Atan2(shootDir.y, shootDir.x) * Mathf.Rad2Deg;
		Quaternion lookRotation = Quaternion.AngleAxis (angle, Vector3.forward);
		GameObject bulletObj = Instantiate (bulletTrailPrefab, firePoint.position, lookRotation) as GameObject;
		BulletTrail trail = bulletObj.GetComponent<BulletTrail> ();
		trail.distance = distance;
		trail.startPos = firePoint.position;
	}

	void shootProjectile () {
		Projectile clone = (Projectile) Instantiate (shotObject, firePoint.position, firePoint.rotation);
		clone.GetComponent<Rigidbody2D> ().AddForce (shootDir * projSpeed);
	}

	void Update(){
		direction = (Target.position - transform.position).normalized;
		float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
		lookRotation = Quaternion.AngleAxis (angle, Vector3.forward);
		transform.rotation = Quaternion.Slerp(transform.rotation,lookRotation,Time.deltaTime * RotateSpeed);
		playerCheck ();
	}
		
	void playerCheck(){
		RaycastHit2D hit = Physics2D.Raycast(firePoint.position, firePoint.right, 100, canBeShot);
		if (hit) {
			string hitLayer = LayerMask.LayerToName (hit.transform.gameObject.layer);
			if (hitLayer != null && hitLayer == "Character") {
				Fire ();
			}
		}
	}

	public void applyDamage(float damage)
	{
		health -= damage;
		if (health <= 0)
			Destroy (gameObject);
	}

}
