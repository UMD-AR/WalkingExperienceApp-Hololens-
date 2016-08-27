using UnityEngine;
using System.Collections;

public class SpawnManagerCode : MonoBehaviour {
	
	public GameObject steve;
	public float time = 5f;
	public Transform[] spawnPoints;
	public int count = 0;
    public int max = 5;
    int spawnPointIndex;
	public float waitTime = 15f;

	void Start () {
		InvokeRepeating ("Spawn", 0, time);
	}

	void Spawn () {
		if (count < max) {
			spawnPointIndex = Random.Range (0, spawnPoints.Length);
			Instantiate (steve, spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation);
			count++;
		} else if (count == max && explosionTrigger.alive == false) {
			InvokeRepeating ("Spawn", waitTime, time * 2);
		}
	}
}