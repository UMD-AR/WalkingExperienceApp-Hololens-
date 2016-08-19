using UnityEngine;
using System.Collections;

public class ZomManager : MonoBehaviour {
	//The Entire MineCraftCharacter with all of its sub-objects
	public GameObject mineCraft;
	//Each time it will spawn 
	public float spawnTime = 3f;
	//What location will they spawn, (fixed on the scene)
	public Transform[] spawnPoints;
	//used to make sure to not spawn the same character when "dead"
	public bool alive;
	// Use this for initialization
	void Start () {
		//Keep looping/calling the spawns
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}
	
	// Update is called once per frame
	void Spawn () {
		if (alive == false) {
			return;
		}
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		//Make MineCraftCharacter appear at specific position
		Instantiate (mineCraft, spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation);
	}
}
