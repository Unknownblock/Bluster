using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject rocketObject;

	public GameObject currentRocket;

	public int timeTilNextSpawn = 5;

	public float timer;

	private void Start()
	{
		timer = 0f;
	}

	private void Update()
	{
		if (currentRocket == null)
		{
			timer += Time.deltaTime;
			Spawn();
		}
	}

	private void Spawn()
	{
		if (timer >= (float)timeTilNextSpawn)
		{
			currentRocket = Object.Instantiate(rocketObject, base.transform.position, base.gameObject.transform.rotation);
			timer = 0f;
		}
	}
}
