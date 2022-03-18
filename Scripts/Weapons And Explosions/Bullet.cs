using UnityEngine;

public class Bullet : MonoBehaviour
{
	[Header("Assignable Variables")]
	public GameObject impactEffect;
	public GameObject bulletHole;

	[Header("Amount Variables")]
	public int damage;
	public float force = 750f;

	private void OnCollisionEnter(Collision other)
	{
		Target target = other.gameObject.GetComponent<Target>(); //See If We've Hit a Normal Target
		ExplodingTarget explodingTarget = other.gameObject.GetComponent<ExplodingTarget>(); //See If We've Hit a Exploding Target
		Rigidbody otherRb = other.gameObject.GetComponent<Rigidbody>(); //See If We've Hit a Rigidbody
		
		if (target != null) //If Hit a Target
		{
			HitMarker.Instance.FadeIn();
			target.TakeDamage(damage); //Hit Damage
			PrefabManager.Instance.DisplayDamagePopUp(damage, other.transform); //Damage Pop Up Instantiation
		}
		
		if (explodingTarget != null) //If Hit an Exploding Target
		{
			HitMarker.Instance.FadeIn();
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
			GameObject hitBulletHole = Instantiate(bulletHole, new Vector3(other.contacts[0].point.x, (float) (other.contacts[0].point.y * 1.0025), other.contacts[0].point.z), Quaternion.LookRotation(-other.contacts[0].normal)); //Instantiate The Bullet Hole
			Destroy(hitBulletHole, 5f); //Destroy The Instantiated Bullet Hole
		}

		GameObject hitImpactEffect = Instantiate(impactEffect, new Vector3(other.contacts[0].point.x, (float) (other.contacts[0].point.y * 1.0025), other.contacts[0].point.z), Quaternion.LookRotation(other.contacts[0].normal)); //Instantiate The Impact Effect
		Destroy(hitImpactEffect, 1f); //Destroy The Instantiated Impact Effect
		Destroy(gameObject); //Destroy The Bullet
	}
}
