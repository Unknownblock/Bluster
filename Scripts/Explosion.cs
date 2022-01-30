using UnityEngine;

public class Explosion : MonoBehaviour
{
	public int damageAmount = 100;
	public float radius;
	public float force;
	public float disAppearingTime = 1;

	private void Start()
	{
		Explode(); //Explosion
	}

	private void Explode()
	{
		Collider[] areaColliders = Physics.OverlapSphere(gameObject.transform.position, radius); //Detecting Objects In an Area
		
		foreach (Collider everyColliders in areaColliders)
		{
			Rigidbody areaRigidBody = everyColliders.GetComponent<Rigidbody>(); //Areas Rigidbody
			Target areaTarget = everyColliders.GetComponent<Target>(); //Areas Normal Target
			ExplodingTarget areaExplodingTarget = everyColliders.GetComponent<ExplodingTarget>(); //Areas Exploding Target
			
			if (areaRigidBody != null && everyColliders.GetComponent<Transform>().gameObject.layer != LayerMask.NameToLayer("Bullet"))
			{
				areaRigidBody.AddExplosionForce(force, transform.position, radius); //Adding Explosion Force
			}
			
			if (areaExplodingTarget != null && areaExplodingTarget != transform.GetComponent<ExplodingTarget>())
			{
				//Damaging The Targets Near The Explosion
				areaExplodingTarget.TakeDamage((int)(damageAmount - Vector3.Distance(areaExplodingTarget.gameObject.transform.position, transform.position)));
			}
			
			if (areaTarget != null)
			{
				//Damaging The Targets Near The Explosion
				areaTarget.TakeDamage((int)(damageAmount - Vector3.Distance(areaTarget.gameObject.transform.position, transform.position)));
			}
		}
		
		//Destroying The Explosion After Some Time
		Destroy(gameObject, disAppearingTime);
	}
}
