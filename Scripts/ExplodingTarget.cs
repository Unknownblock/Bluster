using UnityEngine;

public class ExplodingTarget : MonoBehaviour
{
	[Header("Health Variables")]
	public float health = 100f;

	public float radius;

	public float force;

	public void TakeDamage(int damage)
	{
		health -= damage;
		if (health <= 0f)
		{
			GameObject obj = Object.Instantiate(PrefabManager.Instance.explosion, base.transform.position, Quaternion.identity);
			obj.GetComponent<Explosion>().radius = radius;
			obj.GetComponent<Explosion>().force = force;
			obj.GetComponent<ParticleSystem>().Play();
			Object.Destroy(obj, 2f);
			Object.Destroy(base.gameObject);
		}
	}
}
