using UnityEngine;
using System.Collections;

public class WallSpawner : MonoBehaviour
{
	public GameObject prefabToSpawn;
	public Vector3 spawnLocation;
	public float secondsPerSpawn = 3.0f;
	private float _lastSpawnTime = -1000f;
	public void Update()
	{
		if (Time.time - _lastSpawnTime > secondsPerSpawn)
		{
			_lastSpawnTime = Time.time;

			GameObject go = Instantiate(prefabToSpawn);
			go.transform.position = spawnLocation;
			if (go.GetComponent<Rigidbody>() == null)
				go.AddComponent<Rigidbody>();
		}
	}
}
