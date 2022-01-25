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
		//Spawn The Object For Each Spawn Amount
		for (var i = 0; i < spawnAmount; i++)
		{
			Spawn();
		}
		
		if (spawnedObject == null)
		{
			return;
		}
		
		//List Of Spawned Object
		GameObject[] array = spawnedObject.ToArray();
		
		foreach (GameObject everyObject in array)
		{
			//Removing The Array Part If Null
			if (everyObject == null)
			{
				spawnedObject.Remove(gameObject);
			}
			
			//Setting Every Array Object Active If Not Null
			if (everyObject != null)
			{
				everyObject.SetActive(true);
			}
		}
		
		//Spawning If No Array Part Exists
		if (array.Length == 0)
		{
			canSpawn = true;
		}
		
		//Not Spawning If an Array Exists
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
		
		//Spawn Spreads
		float xSpread = Random.Range(xSpawnSpread, -xSpawnSpread);
		float ySpread = Random.Range(minHeight, maxHeight);
		float zSpread = Random.Range(zSpawnSpread, -zSpawnSpread);

		//Spawning The Objects With Spread
		if (spawnObject != null)
		{
			spawnedObject.Add(Instantiate(spawnObject, gameObject.transform.position + new Vector3(xSpread, ySpread, zSpread), Quaternion.identity, transform));
		}
	}
}
