using UnityEngine;
using System.Collections;

public class Spawn_script : MonoBehaviour {

	public GameObject character;
	public float time = 5f;
	public Transform[] points;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("Spawn_Char", time, time);
	}
	
	// Update is called once per frame
	void Spawn_Char () {
		int index = Random.Range (0, points.Length);
		Instantiate (character, points[index].position, points[index].rotation);
	}
}
