using System.Collections.Generic;
using UnityEngine;

public class MultipleObjectSpawner : MonoBehaviour
{
	public int spawnAmount;

	public float xSpawnSpread;
	public float zSpawnSpread;
	public float minHeight;
	public float maxHeight;

	public GameObject spawnObject;

	public List<GameObject> spawnedObject;

	public bool canSpawn;

	private void Update()
	{
		for (var i = 0; i < spawnAmount; i++)
		{
			Spawn();
		}
		
		if (spawnedObject == null)
		{
			return;
		}
		
		GameObject[] array = spawnedObject.ToArray();
		
		foreach (GameObject everyObject in array)
		{
			if (everyObject == null)
			{
				spawnedObject.Remove(gameObject);
			}
			
			if (everyObject != null)
			{
				everyObject.SetActive(true);
			}
		}
		
		if (array.Length == 0)
		{
			canSpawn = true;
		}
		
		if (array.Length > 0)
		{
			canSpawn = false;
		}
	}

	private void Spawn()
	{
		if (!canSpawn)
		{
			return;
		}
		
		float xSpread = Random.Range(xSpawnSpread, -xSpawnSpread);
		float ySpread = Random.Range(minHeight, maxHeight);
		float zSpread = Random.Range(zSpawnSpread, -zSpawnSpread);

		if (spawnObject != null)
		{
			spawnedObject.Add(Instantiate(spawnObject, gameObject.transform.position + new Vector3(xSpread, ySpread, zSpread), Quaternion.identity, transform));
		}
	}
}
