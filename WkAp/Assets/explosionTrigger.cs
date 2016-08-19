using UnityEngine;
using System.Collections;

public class explosionTrigger : MonoBehaviour
{

	Rigidbody rb;
	public bool collided;
	float t;
	public static float removeTimeLimit = 3f, backUpRemoveTimeLimit = 3 * removeTimeLimit; // in seconds
	public static float minImpulse = 0f;

	// Use this for initialization
	void Start()
	{
		t = 0;
		collided = false;
		rb = GetComponent<Rigidbody>();
	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log ("collided = " + collided);
		// if the collision has happened

		if (collided)
		{
			Debug.Log ("collided Inside");	
			// start the timer
			t += Time.deltaTime;

			// if this piece has stopped moving or it has been to long
			if (rb.velocity.magnitude == 0 || t > backUpRemoveTimeLimit)
			{
				// if this piece has been sitting still for long enough
				if (t > removeTimeLimit)
				{
					// remove this piece
					gameObject.SetActive(false);
				}
			}
		}
	}

	void OnCollisionEnter(Collision collision)
	{

		//Debug.Log (gameObject.name + " collided with " + collision.collider.name + ": collided = " + collided + ": impulse " + collision.impulse.magnitude);
		// if:
		// it collides with the wall
		// it hasn't already collided with something
		// and it hits it hard enough
		if (collision.collider.name.Equals("Wall") && !collided && collision.impulse.magnitude >= minImpulse)
		{
			Debug.Log ("OnCollisionEnter");
			collided = true;
			rb.useGravity = true;
			// create point of explosion between knees
			GameObject torso = GameObject.Find("Torso"), leg = GameObject.Find("Right_Leg");
			Vector3 explosionPoint = new Vector3(torso.transform.position.x, leg.transform.position.y, torso.transform.position.z);
			rb.AddExplosionForce(1000f, explosionPoint, 10f);
		}
	}
}
