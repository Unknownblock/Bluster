using UnityEngine;

public class Target : MonoBehaviour
{
	[Header("Health Variables")]
	public float health = 100f;

	public void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0f)
		{
			Die();
		}
	}

	private void Die()
	{
		Object.Destroy(base.gameObject);
	}
}
