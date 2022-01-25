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

	public void SetShooter(GameObject setShooter)
	{
		shooter = setShooter; //Setting The Shooter
	}
	
	private void OnCollisionEnter(Collision other)
	{
		Target target = other.gameObject.GetComponent<Target>(); //Hit Target Component
		
		PlayerHealth player = other.gameObject.GetComponent<PlayerHealth>(); //Hit Player Health Component
		
		ExplodingTarget explodingTarget = other.gameObject.GetComponent<ExplodingTarget>(); //Hit Exploding Target Component
		
		Rigidbody hitRigidbody = other.gameObject.GetComponent<Rigidbody>(); //Hit Rigidbody Component
		
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			if (player != null)
			{
				player.Damage(damage, shooter); //Damaging The Player
			}
			
			//Blood Effect
			GameObject blood = Instantiate(bloodEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
			
			Destroy(blood, 1f); //Destroy The Blood Effect
		}
		
		else
		{
			//Impact Effect
			GameObject impact = Instantiate(impactEffect, other.contacts[0].point, Quaternion.LookRotation(other.contacts[0].normal));
			
			Destroy(impact, 1f); //Destroy The Impact Effect
		}
		
		if (target != null)
		{
			target.TakeDamage(damage); //Damaging The Target
		}
		
		if (explodingTarget != null)
		{
			explodingTarget.TakeDamage(damage); //Damaging The Exploding Target
		}
		
		if (hitRigidbody != null && other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			hitRigidbody.velocity = Vector3.zero; //Setting The Velocity To Zero
		}
		
		else if (hitRigidbody != null && other.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			hitRigidbody.velocity = Vector3.zero; //Setting The Velocity To Zero
			hitRigidbody.AddForce(force * -other.contacts[0].normal); //Adding Force To The Rigidbody Object
		}
		
		else
		{
			//Spawning The Bullet Hole
			GameObject spawnedBulletHole = Instantiate(bulletHole, other.contacts[0].point * 1.0025f, Quaternion.LookRotation(-other.contacts[0].normal));
			
			Destroy(spawnedBulletHole, 5f); //Destroy The Bullet Hole Effect
		}
		
		Destroy(gameObject, 0.5f); //Destroying The Projectile / Bullet
	}
}
