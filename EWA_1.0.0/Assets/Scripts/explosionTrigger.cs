using UnityEngine;
using System.Collections;

public class explosionTrigger : MonoBehaviour
{

    Rigidbody rb; // this objects rigidbody
    bool steveCollided, alreadyExploding; // boolings to show if steve has collided and is exploding
	public static bool alive = true;

    Transform steve;

    // Use this for initialization
    void Start()
    {
        if (this.name.Equals("Torso"))
        {
            steve = this.transform.parent.parent;
            steveCollided = steve.GetComponent<SteveController>().collided;
        }
        else
        {
            steve = this.transform.parent.parent.parent;
            steveCollided = steve.GetComponent<SteveController>().collided;
        }
        alreadyExploding = false;
        rb = this.GetComponent<Rigidbody>();
    }
    
    // OnCollisionEnter is called when there is a collision between two rigidbodies
    void OnCollisionEnter(Collision collision)
    {
        // if steve hasn't collided with anything yet and collision calling this function is not with another body part or the floor
		if (!steveCollided && !bodyPartOrFloor(collision.collider.name))
        {
			steveCollided = true;
			// Debug.Log ("Kameron");
			if (steve.GetComponent<SteveController> ().state.Equals ("scared")) {
				Debug.Log ("explosion collision detected");
				// steve has now collided!

				steve.GetComponent<SteveController> ().collided = true;

				collision.collider.

			// otherwise, he hit the wall without running
			} else {
				steve.GetComponent<SteveController> ().animator.ResetTrigger ("walk");
				steve.GetComponent<SteveController> ().animator.SetTrigger ("idle");
				steve.GetComponent<SteveController> ().state = "idle";
				steve.GetComponent<SteveController> ().steveHitWallSlowly= true;
				Debug.Log ("steve's state: " + steve.GetComponent<SteveController>().state);
				// and should just hit the wall and not go through
			}
        }
    }

    // this method determines if the string sent to it is one of the
    // body parts of the main character or the floor
    bool bodyPartOrFloor(string s)
    {
        return s.Equals("Head")
            || s.Equals("Torso")
            || s.Equals("Right_Arm")
            || s.Equals("Left_Arm")
            || s.Equals("Right_Leg")
            || s.Equals("Left_Leg")
            || s.Equals("Floor");
    }

    // this method makes the body part this script is attached to explode
    public void explode()
    {
        if (!alreadyExploding)
        {
            alreadyExploding = true;
            rb.useGravity = true;
            alive = false;
            rb.AddExplosionForce(400f, steve.position, 10f);
        }
    }
}