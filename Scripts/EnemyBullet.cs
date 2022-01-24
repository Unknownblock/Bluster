using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
	[Header("Assignable Variables")]
	public GameObject shooter;

	public GameObject impactEffect;

	public GameObject bloodEffect;

	public GameObject bulletHole;

	[Header("Amount Variables")]
	public int damage;

	public float force = 700f;

	private void OnCollisionEnter(Collision other)
	{
		int layer = other.gameObject.layer;
		Physics.IgnoreLayerCollision(base.transform.gameObject.layer, base.gameObject.layer);
		Target component = other.gameObject.GetComponent<Target>();
		PlayerHealth component2 = other.gameObject.GetComponent<PlayerHealth>();
		ExplodingTarget component3 = other.gameObject.GetComponent<ExplodingTarget>();
		Rigidbody component4 = other.gameObject.GetComponent<Rigidbody>();
		if (layer == LayerMask.NameToLayer("Player"))
		{
			GameObject obj = Object.Instantiate(bloodEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
			if (component2 != null)
			{
				component2.Damage(damage, shooter);
			}
			Object.Destroy(obj, 1f);
		}
		else
		{
			Object.Destroy(Object.Instantiate(impactEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)), 1f);
		}
		if (component != null)
		{
			component.TakeDamage(damage);
		}
		if (component3 != null)
		{
			component3.TakeDamage(damage);
		}
		if ((bool)component4 && layer == LayerMask.NameToLayer("Player"))
		{
			component4.velocity = Vector3.zero;
		}
		else if ((bool)component4 && layer != LayerMask.NameToLayer("Player"))
		{
			component4.velocity = Vector3.zero;
			component4.AddForce(force * -other.contacts[0].normal);
		}
		else
		{
			Object.Destroy(Object.Instantiate(bulletHole, other.contacts[0].point * 1.0025f, Quaternion.LookRotation(-other.contacts[0].normal)), 5f);
		}
		Object.Destroy(base.gameObject);
	}
}
