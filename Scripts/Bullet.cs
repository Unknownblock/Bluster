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
		
		if (target != null) //If Hit a Target
		{
			target.TakeDamage(damage); //Hit Damage
			PrefabManager.Instance.DisplayDamagePopUp(damage, other.transform); //Damage Pop Up Instantiation
		}
		
		if (explodingTarget != null) //If Hit an Exploding Target
		{
			explodingTarget.TakeDamage(damage); //Hit Damage
			PrefabManager.Instance.DisplayDamagePopUp(damage, other.transform); //Damage Pop Up Instantiation
		}
		
		if (otherRb != null) //If Hit a Rigidbody
		{
			otherRb.velocity = Vector3.zero; //If Hit Reset The Velocity
			otherRb.AddForceAtPosition(force * transform.forward, gameObject.transform.position); //Add The Custom Force To The Rigidbody
		}
		
		else
		{
			GameObject hitBulletHole = Instantiate(bulletHole, other.contacts[0].point * 1.0025f, Quaternion.LookRotation(-other.contacts[0].normal)); //Instantiate The Bullet Hole
			Destroy(hitBulletHole, 5f); //Destroy The Instantiated Bullet Hole
		}

		GameObject hitImpactEffect = Instantiate(impactEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal)); //Instantiate The Impact Effect
		Destroy(hitImpactEffect, 1f); //Destroy The Instantiated Impact Effect
		Destroy(gameObject); //Destroy The Bullet
	}
}
