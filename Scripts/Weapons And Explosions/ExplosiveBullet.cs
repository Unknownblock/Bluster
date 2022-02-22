using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
	[Header("Amount Variables")]
	public float radius;
	public float force;

	private void FixedUpdate()
	{
		//Changing The Rotation By Velocity
		transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity, Vector3.up);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			//Instantiating Explosion Prefab
			GameObject explosion = Instantiate(PrefabManager.Instance.explosion, transform.position, Quaternion.identity);
			
			explosion.GetComponent<Explosion>().radius = radius; //Setting The Radius
			explosion.GetComponent<Explosion>().force = force; //Setting The Radius
			
			Destroy(gameObject); //Destroying The Projectile / Bullet
		}
	}
}
