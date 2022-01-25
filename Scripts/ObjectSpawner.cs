using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
	public GameObject rocketObject;

	public GameObject currentRocket;

	public int timeTilNextSpawn = 5;

	public float timer;

	private void Start()
	{
		timer = 0f; //Setting The Timer To Zero On Start
	}

	private void Update()
	{
		if (currentRocket == null)
		{
			timer += Time.deltaTime; //Adding The Time
			Spawn();
		}
	}

	private void Spawn()
	{
		if (timer >= timeTilNextSpawn) //If The Time Passed The Wanted Time
		{
			//Spawn
			currentRocket = Instantiate(rocketObject, transform.position, gameObject.transform.rotation);
			
			timer = 0f; //Setting The Timer To Zero
		}
	}
}
