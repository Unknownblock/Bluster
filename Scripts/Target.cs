using UnityEngine;

public class Target : MonoBehaviour
{
	[Header("Health Variables")]
	public float health = 100f;

	public void TakeDamage(int damage)
	{
		//Taking Damage
		health -= damage;
		if (health <= 0f)
		{
			Destroy();
		}
	}

	private void Destroy()
	{
		//Destroying The Object
		Destroy(gameObject);
	}
}
