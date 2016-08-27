﻿//Most Recent Version
using UnityEngine;
using System.Collections;
/*
 * This script controls the movements of the A.I. characters
 * This script must be attached to the A.I. model
 * Script works off the assumption the Camera is directly on top of the player
 * Scipts does not take into account any y-axis movement
 */ 

public class SteveController : MonoBehaviour {

	public Animator animator;
	Camera theCamera;
	float closeDistance = 12.0f; // distance at which the A.I. will run from the player
	float medDistance = 70f; // distance at which the A.I. will run toward the player
	public float actionTimer; // minimum time to perform an idling action i.e. idle or walk
	float thetaCorrection = 180f; // necessary as the Minecraft is orientied so its back is facing 180 degrees
	float rotationSpeed = 30f; // rotation speed during idle
	float noticeRotationSpeed = 120f; // rotation speed when player detected
	float desiredTheta;
	float runSpeed = 14f;
	float walkSpeed = 2f;
	float currentSpeed = 0f;
	float acceleration;
	float fov = 60;
	float walkChance = .005f; // 1 / walkchance is the chance the A.I. will stop idling and turn and walk
	public string state = "idle";
	string playerDependent = "none";
	float cameraCorrection = 90f; // For some reason 0 degrees points the camera along the positive z axis (as opposed to the x)
	float tooClose = 7f;
	float previousX, previousZ; // holds the previous position of the Camera, used to prevent the A.I. from getting to close
	public bool steveHitWallSlowly;

    public bool collided = false;
    GameObject[] bodyParts;
    float timeAfterCollision;

	void Start() {

		animator = GetComponent<Animator>();
		theCamera = Camera.main;
		acceleration = walkSpeed / 10f;

        bodyParts = new GameObject[6];
        bodyParts[0] = this.transform.Find("Armature").Find("Torso").gameObject;
        for (int i = 1; i < bodyParts.Length; i++)
        {
            bodyParts[i] = bodyParts[0].transform.GetChild(i - 1).gameObject;
        }
        
        collided = false;
		steveHitWallSlowly = false;
        timeAfterCollision = 0;

	}

	/*
     * Handles all the triggers and movements of the A.I.
     * Behavior:
     * If the player is not looking at the A.I. The A.I. will idle around
     * If the player is looking at the A.I. and the A.I. is close enough, the A.I. will notice the player, then run away
     * If the player gets to far away from the A.I. and is not looking at the A.I.
     */
	void Update() {
        if (collided)
        {
            explosion();
        }
        // otherwise, run his AI
        else
        {
            
            //check to see if the character is too close to the player and not facing it and the A.I. is not already running away or about to run away
            if (Vector3.Distance(theCamera.transform.position, this.transform.position) < tooClose && state
                != "close" && state != "scared" && (state != "turn" || playerDependent == "none"))
            {
                state = "close";
                previousX = theCamera.transform.position.x;
                previousZ = theCamera.transform.position.z;
                animator.ResetTrigger("walk");
                animator.ResetTrigger("idle");
                actionTimer = 0f;
                Debug.Log("State changed to close");
            }
            //can the player to close to the A.I. and can the player see the A.I.
            if (Vector3.Distance(theCamera.transform.position, this.transform.position) < closeDistance && state != "scared" && inFOV())
            {
                //change the state to turn
                state = "turn";
                if (playerDependent == "none")
                {
                    playerDependent = "toward";
                    animator.ResetTrigger("idle");
                }
            }
            //check to see if the character is too far away from the player
            else if (Vector3.Distance(theCamera.transform.position, this.transform.position) > medDistance && state != "run" && !inFOV())
                state = "run";
            //ensures the character does not turn toward the character by accidnet
            else
                playerDependent = "none";
            //Handles all the states the character can be in:
            // walking, running, turning, and idling
            switch (state)
            {
                //stick to the player
                case "close":
                    animator.SetTrigger("idle");
                    //has the player moved since the last updateer
                    if (previousZ - theCamera.transform.position.z != 0 || previousX - theCamera.transform.position.x != 0)
                    {
                        //has the player gotten closer to the A.I.
                        if (Vector3.Distance(this.transform.position, new Vector3(previousX, theCamera.transform.position.y, previousZ)) >= Vector3.Distance(this.transform.position, theCamera.transform.position)) {

                            // Debug.Log("movement");
                            //if so move the A.I. so that it is the same distance from the player as the last update, unless it has hit the wall slowly, in which case thier position will not change
							if (!steveHitWallSlowly) {
								this.transform.position = new Vector3(this.transform.position.x + theCamera.transform.position.x - previousX, this.transform.position.y, this.transform.position.z + theCamera.transform.position.z - previousZ);
							}
						}
                        previousX = theCamera.transform.position.x;
                        previousZ = theCamera.transform.position.z;
                    }
                    //has the player moved far enough away to leave the close state
                    if (Vector3.Distance(this.transform.position, theCamera.transform.position) > tooClose)
                    {
                        state = "idle";
                        actionTimer = 3f;
                    }
                    break;
                //walk in the direction the A.I. is currently facing
			case "walk":
                    //decrease walkTimer
				if (actionTimer > Time.deltaTime) {
					if (!steveHitWallSlowly) {
						animator.SetTrigger ("walk");
						actionTimer -= Time.deltaTime;
						accelerate (walkSpeed);
						this.transform.position = new Vector3 (this.transform.position.x + Time.deltaTime * currentSpeed * Mathf.Cos ((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * currentSpeed * Mathf.Sin ((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180));
					}
				}
                    //change state to idle
                    else {
                        actionTimer = 3f;
                        state = "idle";
                        // Debug.Log("Changing states from Walk to Idle");
                        animator.ResetTrigger("walk");
                    }
                    break;
                //run towards the player if the player is to far away and not looking at the A.I.
                case "run":
                    animator.SetTrigger("run");
                    //stop running if the player can see the A.I.
                    if (inFOV())
                    {
                        animator.ResetTrigger("run");
                        actionTimer = .2f;
                        // Debug.Log("Changing states from run to walk");
                    }
                    desiredTheta = processAngle(thetaCorrection - angleBetween());
                    //check to see if the character is still to far away
                    if (Vector3.Distance(this.transform.position, theCamera.transform.position) > (medDistance - closeDistance) / 2f)
                    {
                        transform.localRotation = Quaternion.Euler(0, desiredTheta, 0);
                        currentSpeed = runSpeed;
                        this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * currentSpeed * Mathf.Cos((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * currentSpeed * Mathf.Sin((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180));
                    }
                    // change state to walk
                    else {
                        state = "walk";
                        actionTimer = 3f;
                        // Debug.Log("Changing states from run to walk");
                        animator.ResetTrigger("run");
                    }
                    break;
                //sit still for awhile and occassionaly turn and walk somewhere
                case "idle":
                    animator.SetTrigger("idle");
                    currentSpeed = 0f;
                    if (actionTimer > Time.deltaTime)
                    {
                        actionTimer -= Time.deltaTime;
                    }
                    else {
                        actionTimer = 0;
                        //1 / walkChance is the chance the character will turn and walk after the action Timer has expired
                        //if walkChance is selected, change states to turn player Indepent
                        if (Random.Range(0, 1f) < walkChance)
                        {
                            state = "turn";
                            playerDependent = "none";
                            desiredTheta = processAngle((this.transform.eulerAngles.y + Random.Range(-30, 30)));
                            // Debug.Log("Changing states from idle to turn");
                            animator.ResetTrigger("idle");
                        }
                    }
                    break;
                //turn in a specified direction
                case "turn":
                    //playerDependent determines which direction the character will turn and what animation will play during the turn
                    switch (playerDependent)
                    {
                        //turn toward the player to 'notice' them
                        case "toward":
                            animator.SetTrigger("walk");
                            desiredTheta = processAngle(thetaCorrection - angleBetween());
                            rotateObject(desiredTheta, noticeRotationSpeed);
                            //check if the current angle is close enough to the desiredAngle
                            //if so change states to turn away
                            if (Mathf.Abs(processAngle(this.transform.eulerAngles.y) - desiredTheta) < 3)
                            {
                                state = "turn";
                                playerDependent = "away";
                                // Debug.Log("Changing states from turn toward to turn away");
                                animator.ResetTrigger("walk");
                            }
                            break;
                        //turn away from the player after noticing them
                        case "away":
                            animator.SetTrigger("scared");
                            desiredTheta = processAngle(thetaCorrection - angleBetween() + 180);
                            rotateObject(processAngle(desiredTheta), noticeRotationSpeed * 1.5f);
                            //check if the current angle is close enough to the desiredAngle
                            //if so change states to scared run
                            if (Mathf.Abs(processAngle(this.transform.eulerAngles.y) - desiredTheta) < 6)
                            {
                                state = "scared";
                                // Debug.Log("Changing states from turn away to scared run");
                                animator.ResetTrigger("scared");
                            }
                            break;
                        //turn randomly while idling
                        case "none":
                            animator.SetTrigger("walk");
                            rotateObject(desiredTheta, rotationSpeed);
                            //check to see if the current angle is close enough to the desiredAngle
                            //if so change staets to walk
                            if (Mathf.Abs(this.transform.eulerAngles.y - desiredTheta) < 5)
                            {
                                state = "walk";
                                actionTimer = Random.Range(2, 10);
                                // Debug.Log("Changing states from turn neutral to walk");
                                animator.ResetTrigger("walk");
                            }
                            break;
                    }
                    break;
                //run away from the player until a certain distance has been achieved
                case "scared":
                    animator.SetTrigger("scared");
                    //if the distance has been achieved, change states to walk
                    if (Vector3.Distance(this.transform.position, theCamera.transform.position) > (medDistance - closeDistance) / 2f)
                    {
                        state = "walk";
                        actionTimer = 5f; // walk in the same direction for 5 additional seconds
                        playerDependent = "none";
                        // Debug.Log("Changing states from scared run to walk");
                        animator.ResetTrigger("scared");
                    }
                    //continue running
                    else {
                        accelerate(runSpeed);
                        this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * currentSpeed * Mathf.Cos((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180), this.transform.position.y, this.transform.position.z + Time.deltaTime * currentSpeed * Mathf.Sin((thetaCorrection - this.transform.eulerAngles.y) * Mathf.PI / 180));
                    }
                    break;

            }
        }

	}

	/*
     * rotates the character to the desiredAngle at the given speed in degrees / second 
     * Requires an angle between 0 and 360 degrees
     */
	void rotateObject(float desiredAngle, float speed) {
		//turn counterclockwise
		if (processAngle(desiredAngle - this.transform.eulerAngles.y) < 180) {
			this.transform.Rotate(Vector3.up * Time.deltaTime * speed);
		}
		//turn clockwise
		else {
			this.transform.Rotate(Vector3.down * Time.deltaTime * speed);
		}
	}

	/*
     * Converts all angles to angles between 0 and 360 
     * Requires an angle in degrees
     */
	float processAngle(float theAngle) {
		if (theAngle > 360) {
			return processAngle(theAngle - 360);
		}
		else if (theAngle < 0) {
			return processAngle(theAngle + 360);
		}
		else {
			return theAngle;
		}
	}

	/*
     * Converts all angles to angles between -180 and 180
     * Used in the case where the angles cross from 0 to 359 degrees
     * Requires an angle in degrees
     */
	float processAngle2(float theAngle) {
		if (theAngle > 180) {
			return processAngle2(theAngle - 360);
		}
		else if (theAngle < -180) {
			return processAngle2(theAngle + 360);
		}
		else {
			return theAngle;
		}
	}

	/*
     * Returns the angle between the character and the main camera (attached to the player) 
     * Assumes the camera is attached to the player
     */
	float angleBetween() {
		float xDis = theCamera.transform.position.x - this.transform.position.x;
		float zDis = theCamera.transform.position.z - this.transform.position.z;
		float angle = -Mathf.PI;
		float bestAngle = 0f;
		float distance = int.MaxValue;
		float hypotenuse = Mathf.Sqrt(Mathf.Pow(xDis, 2) + Mathf.Pow(zDis, 2));
		//loop through all angles between -Pi and Pi at rate of .01 radians to determine which is the closest fit
		while (angle < Mathf.PI) {
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
			angle += .1f;
		}
		return bestAngle * 180 / Mathf.PI;
	}


    /*
     * Returns true if the A.I. is in theCamera's Field of View 
     */
    bool inFOV() {
        Vector3 isSeen = theCamera.WorldToViewportPoint(this.transform.position);
        return isSeen.x >= 0 && isSeen.x <= 1 && isSeen.y >= 0 && isSeen.y <= 1 && isSeen.z >= 0;
    }

    /*
     * Increase or decrease current speed until close enought to desired speed
     */
    void accelerate(float desiredSpeed) {
		if (desiredSpeed > currentSpeed && Mathf.Abs(desiredSpeed - currentSpeed) > .3f) {
			currentSpeed += desiredSpeed * Time.deltaTime;
		}
		else if (desiredSpeed < currentSpeed && Mathf.Abs(desiredSpeed - currentSpeed) > .3f) { 
			currentSpeed -= desiredSpeed * Time.deltaTime;
		}
	}

    // makes character explode upon impact with wall
    void explosion()
    {
        // stop whatever the animator is playing
        animator.Stop();

        // start the timer
        timeAfterCollision += Time.deltaTime;

        // for each body part
        foreach (GameObject part in bodyParts)
        {
            part.GetComponent<explosionTrigger>().explode();
        }

        if (timeAfterCollision > 6f) // if 6 seconds have passed
        {
            // Debug.Log("dead");

            // decrement the number of instances of steves
           GameObject.Find("SpawnManager").GetComponent<SpawnManagerCode>().count--;
            
            // remove this instance of steve
            gameObject.SetActive(false);

        }
    }
}
