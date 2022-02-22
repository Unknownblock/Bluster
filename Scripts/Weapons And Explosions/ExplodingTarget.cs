using UnityEngine;

public class ExplodingTarget : MonoBehaviour
{
	[Header("Health Variables")]
	public float health = 100f;

	public float radius;

	public float force;

	public void TakeDamage(int damage)
	{
		health -= damage; //Damaging This Object
		
		if (health <= 0f)
		{
			//Instantiating The Explosion
			GameObject explosion = Instantiate(PrefabManager.Instance.explosion, transform.position, Quaternion.identity);
			
			explosion.GetComponent<Explosion>().radius = radius; //Setting Explosions Radius
			explosion.GetComponent<Explosion>().force = force; //Setting Explosions Force To Other Object
			explosion.GetComponent<ParticleSystem>().Play(); //Playing The Explosion Particle Effect
			
			Destroy(explosion, 2f); //Destroying The Explosion Effect
			
			Destroy(gameObject); //Destroying The Object
		}
	}
}
