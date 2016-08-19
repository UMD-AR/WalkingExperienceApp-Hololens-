using UnityEngine;
using System.Collections;

public class PlayerMov : MonoBehaviour {


	public float speed = 6f;
	Vector3 movement;
	Animator anim;
	Rigidbody playerRigBod;
	int floorMask;
	float camRayLength = 100f;

	void Awake(){
		floorMask = LayerMask.GetMask ("Floor");
		anim = GetComponent<Animator> ();
		playerRigBod = GetComponent<Rigidbody> ();
	}

	void FixedUpdate(){
		float hor = Input.GetAxisRaw ("Horizontal");
		float ver = Input.GetAxisRaw ("Vertical");

		Move (hor, ver);
		Turning ();
		Animating (hor, ver);
	}


	void Move (float hor, float ver){
		movement.Set (hor, 0f, ver);
		movement = movement.normalized * speed * Time.deltaTime;

		playerRigBod.MovePosition (transform.position + movement);
	}

	void Turning(){
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit floorHit;
		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
			Vector3 playerToMouse = floorHit.point - transform.position;
			playerToMouse.y = 0f;

			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
			playerRigBod.MoveRotation (newRotation);
		}
	}

	void Animating(float hor, float ver){
		bool walking = hor != 0f || ver != 0f;

	}
}
