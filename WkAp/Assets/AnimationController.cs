using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour {

		Animator animator;
		Camera theCamera;
		float closeDistance = 10f; // distance at which the A.I. will run from the player
		float medDistance = 70f; // distance at which the A.I. will run toward the player
		public float actionTimer; // minimum time to perform an idling action i.e. idle or walk
		float thetaCorrection = Mathf.PI; // necessary as the Minecraft is orientied so its back is facing 180 degrees
		float rotationSpeed = 50f; // rotation speed during idle
		float noticeRotationSpeed = 150f; // rotation speed when player detected
		float desiredTheta;
		float runSpeed = 5f; 
		float walkSpeed = 2f;
		float turnAngle;
		int walkChance = 200; // 1 / walkchance is the chance the A.I. will stop idling and turn and walk
		string state = "idle";
		string  playerDependent = "none";
		public bool triggerSet = false; // prevents triggers from being triggered twice

	Vector3 pos;
	// Rigidbody rb;

	GameObject head, torso, rArm, lArm, rLeg, lLeg;

		// Use this for initialization
		void Start () {
			animator = GetComponent<Animator>();
			theCamera = Camera.main;

		pos = transform.position;

		head = GameObject.Find("Head");
		torso = GameObject.Find("Torso");
		rArm = GameObject.Find("Right_Arm");
		lArm = GameObject.Find("Left_Arm");
		rLeg = GameObject.Find("Right_Leg");
		lLeg = GameObject.Find("Left_Leg");
		}

		// Update is called once per frame
		/*
     * Handles all the triggers and movements of the A.I. 
     */
		void Update () {
		
		// if none of the pieces exist any more
		if (!head.activeSelf &&
			!torso.activeSelf &&
			!rArm.activeSelf &&
			!lArm.activeSelf &&
			!rLeg.activeSelf &&
			!lLeg.activeSelf)
		{
			//Debug.Log ("not active");
			// remove this instance of steve as well
			gameObject.SetActive(false);
		}


			//check to see if the character is close to the player
			if (Vector3.Distance(theCamera.transform.position, this.transform.position) < closeDistance && state != "scared") {
				//change the state to turn
				state = "turn";
				if (playerDependent == "none") {
					playerDependent = "toward";
				}
			}
			//check to see if the character is too far away from the player
			else if (Vector3.Distance(theCamera.transform.position, this.transform.position) > medDistance && state != "run") {
				state = "run";
			}
			//ensures the character does not turn toward the character by accidnet
			else {
				playerDependent = "none";
			}

			//Handles all the states the character can be in:
			// walking, running, turning, and idling
			switch (state) {
			case "walk":
				//decrease walkTimer
				if (actionTimer > Time.deltaTime) {
					animator.SetTrigger("walk");
					actionTimer -= Time.deltaTime;
					this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * walkSpeed * Mathf.Cos(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * walkSpeed * Mathf.Sin(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180));
				}
				//change state to idle
				else {
					actionTimer = 3f;
					state = "idle";
					//Debug.Log("Changing states from Walk to Idle");
					animator.ResetTrigger("walk");
				}
				break;

			case "run":
				animator.SetTrigger("run");
				desiredTheta = processAngle(thetaCorrection - angleBetween());
				//check to see if the character is still to far away
				if (Vector3.Distance(this.transform.position, theCamera.transform.position) > (medDistance - closeDistance) / 2f) {
					transform.localRotation = Quaternion.Euler(0, desiredTheta * 180 / Mathf.PI, 0);
					this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * runSpeed * Mathf.Cos(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * runSpeed * Mathf.Sin(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180));
				}
				// change state to walk
				else {
					state = "walk";
					actionTimer = 3f;
					//Debug.Log("Changing states from run to walk");
					animator.ResetTrigger("run");
				}
				break;
			case "idle":
				animator.SetTrigger("idle");
				if (actionTimer > Time.deltaTime) {
					actionTimer -= Time.deltaTime;
				}
				else {
					actionTimer = 0;
					//1 / walkChance is the chance the character will turn and walk after the action Timer has expired
					//if walkChance is selected, change states to turn player Indepent
					if (Random.Range(0, walkChance) == walkChance - 1) {
						state = "turn";
						playerDependent = "none";
						desiredTheta = processAngle(thetaCorrection - (this.transform.eulerAngles.y + Random.Range(-30, 30) * Mathf.PI / 180f));
						//Debug.Log("Changing states from idle to turn");
						animator.ResetTrigger("idle");
					}
				}
				break;

			case "turn":
				//playerDependent determines which direction the character will turn and what animation will play during the turn
				switch (playerDependent) {
				case "toward":
					animator.SetTrigger("walk");
					desiredTheta = processAngle(thetaCorrection - angleBetween());
					rotateObject(desiredTheta, noticeRotationSpeed);
					//check if the current angle is close enough to the desiredAngle
					//if so change states to turn away
					if (Mathf.Abs(processAngle(this.transform.eulerAngles.y * Mathf.PI / 180f) - processAngle(desiredTheta)) < .1) {
						state = "turn";
						playerDependent = "away";
						//Debug.Log("Changing states from turn toward to turn away");
						animator.ResetTrigger("walk");
					}
					break;
				case "away":
					animator.SetTrigger("scared");
					desiredTheta = processAngle(thetaCorrection - angleBetween() + Mathf.PI);
					rotateObject(processAngle(desiredTheta), noticeRotationSpeed * 2f);
					//check if the current angle is close enough to the desiredAngle
					//if so change states to scared run
					if (Mathf.Abs(processAngle(this.transform.eulerAngles.y * Mathf.PI / 180f) - processAngle(desiredTheta)) < .1) {
						state = "scared";
						//Debug.Log("Changing states from turn away to scared run");
						animator.ResetTrigger("scared");
					}
					break;
				case "none":
					animator.SetTrigger("walk");
					rotateObject(processAngle(desiredTheta), rotationSpeed);
					//check to see if the current angle is close enough to the desiredAngle
					//if so change staets to walk
					if (Mathf.Abs(processAngle(this.transform.eulerAngles.y * Mathf.PI / 180f) - processAngle(desiredTheta)) < .1) {
						state = "walk";
						actionTimer = Random.Range(2, 10);
						//Debug.Log("Changing states from turn neutral to walk");
						animator.ResetTrigger("walk");
					}
					break;
				}
				break;

			case "scared":
				animator.SetTrigger("scared");
				//check to see if the character is far enough away from the player
				if (Vector3.Distance(this.transform.position, theCamera.transform.position) > (medDistance - closeDistance) / 2f) {
					state = "walk";
					actionTimer = 5f;
					playerDependent = "none";
					//Debug.Log("Changing states from scared run to walk");
					animator.ResetTrigger("scared");
				}
				else {
					this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * runSpeed * Mathf.Cos(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * runSpeed * Mathf.Sin(thetaCorrection - this.transform.eulerAngles.y * Mathf.PI / 180));
				}
				break;

			}

		}

		/*
     * rotates the character to the desiredAngle at the given speed in Radians / second 
     */
		void rotateObject(float desiredAngle, float speed) {
			if (processAngle(desiredAngle - this.transform.eulerAngles.y  * Mathf.PI / 180) < Mathf.PI) {
				//  Debug.Log(desiredAngle);
				this.transform.Rotate(Vector3.up * Time.deltaTime * speed);
			}
			else {
				//   Debug.Log("Rotating down");
				this.transform.Rotate(Vector3.down * Time.deltaTime * speed);
			}
		}

		/*
     * Converts all angles to angles between 0 and 2 Pi 
     */
		float processAngle(float theAngle) {
			if (theAngle > 2 * Mathf.PI) {
				return processAngle(theAngle - 2 * Mathf.PI);
			}
			else if (theAngle < 0) {
				return processAngle(theAngle + 2 * Mathf.PI);
			}
			else {
				return theAngle;
			}
		}

		/*
     * Returns the angle between the character and the main camera (attached to the player) 
     */
		float angleBetween() {
			float xDis = theCamera.transform.position.x - this.transform.position.x;
			float zDis = theCamera.transform.position.z - this.transform.position.z;
			float angle = -Mathf.PI;
			float bestAngle = 0f;
			float distance = int.MaxValue;
			float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDis, 2) + Mathf.Pow(zDis, 2));
			//loop through all angles between -Pi and Pi at rate of .01 radians to determine which is the closest fit
			while(angle <  Mathf.PI) {
				float x = Mathf.Cos(angle) * hypotenuse;
				float z = Mathf.Sin(angle) * hypotenuse;
				float xpos = this.transform.position.x + x;
				float zpos = this.transform.position.z + z;
				float curDistance = Vector3.Distance(theCamera.transform.position, new Vector3(xpos, theCamera.transform.position.y, zpos));
				//if this angle is better then the previous best, update it
				if (curDistance < distance) {
					distance = curDistance;
					bestAngle = angle;
				}
				angle += .01f;
			}
			return bestAngle;
		}
}
