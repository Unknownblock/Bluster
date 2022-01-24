using UnityEngine;

public class Lava : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			Debug.Log("Hit Lava");
		}
	}
}
