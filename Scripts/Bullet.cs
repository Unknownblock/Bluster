using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Header("Assignable Variables")]
	public GameObject impactEffect;
	public GameObject bulletHole;

	[Header("Amount Variables")]
	public int damage;
	public float force = 700f;

	private void OnCollisionEnter(Collision other)
	{
		Target target = other.gameObject.GetComponent<Target>(); //See If We've Hit a Normal Target
		ExplodingTarget explodingTarget = other.gameObject.GetComponent<ExplodingTarget>(); //See If We've Hit a Exploding Target
		Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>(); //See If We've Hit a Rigidbody
		
		if (target != null)
		{
			target.TakeDamage(damage);
			PrefabManager.Instance.DisplayDamagePopUp(damage, other.transform);
		}
		
		if (explodingTarget != null)
		{
			explodingTarget.TakeDamage(damage);
			PrefabManager.Instance.DisplayDamagePopUp(damage, other.transform);
		}
		
		if (otherRb != null)
		{
			otherRb.velocity = Vector3.zero;
			otherRb.AddForce(force * -other.contacts[0].normal);
		}
		
		else
		{
			Destroy(Instantiate(bulletHole, other.contacts[0].point * 1.0025f, Quaternion.LookRotation(-other.contacts[0].normal)), 5f);
		}
		
		Destroy(Instantiate(impactEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)), 1f);
		Destroy(gameObject);
	}
}
