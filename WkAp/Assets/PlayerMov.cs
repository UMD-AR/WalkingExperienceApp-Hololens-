using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour {

	//Speed of sphere movement
	public float speed = 6f;
	//will be used to reference the three vectors of the sphere
	Vector3 movement;

	Rigidbody playerRigBod;
	int floorMask;
	float camRayLength = 100f;

	void Awake(){
		floorMask = LayerMask.GetMask ("Floor");
		playerRigBod = GetComponent<Rigidbody> ();
	}

	void FixedUpdate(){
		float hor = Input.GetAxisRaw ("Horizontal");
		float ver = Input.GetAxisRaw ("Vertical");

		Move (hor, ver);
//		Turning ();
	}


	void Move (float hor, float ver){
		
		movement.Set (hor, 0f, ver);
		movement = movement.normalized * speed * Time.deltaTime;

		playerRigBod.MovePosition (transform.position + movement);
	}

//	void Turning(){

//		//imagine laser beam from camera throught the sphere and reaching the floor
//		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
//		RaycastHit floorHit;
//		if (Physics.Raycast (camRay, out floorHit, camRayLength, floorMask)) {
//			Vector3 playerToMouse = floorHit.point - transform.position;
//			playerToMouse.y = 0f;
			//
//			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);
//			playerRigBod.MoveRotation (newRotation);
//		}
//	}

}
