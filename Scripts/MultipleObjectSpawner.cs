using System.Collections.Generic;
using UnityEngine;

public class MultipleObjectSpawner : MonoBehaviour
{
	public int spawnAmount;

	public float spawnSpread;

	public GameObject spawnObject;

	public List<GameObject> spawnedObject;

	public bool canSpawn;

	private void Update()
	{
		int num = 0;
		for (int i = 0; i < spawnAmount; i++)
		{
			Spawn();
		}
		if (spawnedObject == null)
		{
			return;
		}
		GameObject[] array = spawnedObject.ToArray();
		foreach (GameObject gameObject in array)
		{
			if (gameObject == null)
			{
				spawnedObject.Remove(gameObject);
				num++;
			}
			if (gameObject != null)
			{
				gameObject.SetActive(value: true);
			}
		}
		if (num == spawnedObject.Count)
		{
			canSpawn = true;
		}
		if (num < spawnedObject.Count)
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
		if (spawnObject != null)
		{
			spawnedObject.Add(Object.Instantiate(spawnObject, base.gameObject.transform.position, Quaternion.identity, base.transform));
		}
		float x = Random.Range(spawnSpread, 0f - spawnSpread);
		float z = Random.Range(spawnSpread, 0f - spawnSpread);
		foreach (GameObject item in spawnedObject)
		{
			if (item != null)
			{
				item.transform.position = base.transform.position + new Vector3(x, base.gameObject.transform.position.y, z);
			}
		}
	}
}
