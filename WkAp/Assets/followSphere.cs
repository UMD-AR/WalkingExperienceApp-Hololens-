using UnityEngine;
using System.Collections;

public class followSphere : MonoBehaviour {

	//location of target (sphere)
	public Transform targ;
	//constant that will be used to smooth out the frame per second movement
	public float smoothing = 5f;

	Vector3 offset;
	// Use this for initialization
	void Start () {
		//Where the camera will start relative to the target (sphere)
		offset = transform.position - targ.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		//3D vector calculated, for moving the camera with sphere
		Vector3 targetCamPos = targ.position + offset /4;
		//move the camera depending on the coordinates of the sphere. (move the camera with sphere). 
		//The smoothing times the delta time (frames per second), will help with "natural" transitions per frame. 
		transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
	}
}
