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
            spawnPointIndex = Random.Range(0, spawnPoints.Length);
            Instantiate (steve, spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation);
			count++;

		if (count == max) {
			Debug.Log ("MAX");
			CancelInvoke ("Spawn");
			count++;
		}

		if (count > max && explosionTrigger.alive == false) {
			Debug.Log ("Explosion");
			Invoke ("Spawn", waitTime);
			count = 5;
		}
	}
}