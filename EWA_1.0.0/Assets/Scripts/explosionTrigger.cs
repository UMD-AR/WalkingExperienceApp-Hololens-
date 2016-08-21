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
            // steve has now collided!
            steveCollided = true;
            steve.GetComponent<SteveController>().collided = steveCollided;
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
            //GameObject torso = GameObject.Find("Torso"), leg = GameObject.Find("Right_Leg"); // create point of explosion between knees
            //Vector3 explosionPoint = new Vector3(torso.transform.position.x, leg.transform.position.y, torso.transform.position.z);
            rb.AddExplosionForce(400f, steve.position, 10f);
        }
    }
}