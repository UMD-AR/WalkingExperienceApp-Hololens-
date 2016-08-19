﻿using UnityEngine;
using System.Collections;

public class SpawnManagerCode : MonoBehaviour {
	
	public GameObject steve;
	public float time = 5f;
	public Transform[] spawnPoints;
	private int count = 0;
	public int max = 10;

	void Start () {
		InvokeRepeating ("Spawn", 0, time);
	}

	void Spawn () {
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);
		Instantiate (steve, spawnPoints [spawnPointIndex].position, spawnPoints [spawnPointIndex].rotation);
		count++;

		if (count >= max) {
			CancelInvoke ();
		}
	}
}
