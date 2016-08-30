using UnityEngine;
using System.Collections;

public class explosionTrigger : MonoBehaviour
{

    Rigidbody rb; // this objects rigidbody
    bool alreadyExploding; // boolings to show if steve is exploding
	public static bool alive = true;

    Transform steve;

    // Use this for initialization
    void Start()
    {
        if (this.name.Equals("Torso"))
        {
            steve = this.transform.parent.parent;
        }
        else
        {
            steve = this.transform.parent.parent.parent;
        }
        alreadyExploding = false;
        rb = this.GetComponent<Rigidbody>();
    }

    // this method makes the body part this script is attached to explode
    public void explode()
    {
        if (!alreadyExploding)
        {
            alreadyExploding = true;
            rb.useGravity = true;
            alive = false;
            rb.AddExplosionForce(200f, steve.position, 10f);
        }
    }
}