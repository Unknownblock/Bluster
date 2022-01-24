using UnityEngine;

public class Explosion : MonoBehaviour
{
	public float radius;

	public float force;

	private void Start()
	{
		Explode();
	}

	private void Explode()
	{
		Collider[] array = Physics.OverlapSphere(base.gameObject.transform.position, radius);
		foreach (Collider collider in array)
		{
			Rigidbody component = collider.GetComponent<Rigidbody>();
			Target component2 = collider.GetComponent<Target>();
			ExplodingTarget component3 = collider.GetComponent<ExplodingTarget>();
			if (component != null && collider.GetComponent<Transform>().gameObject.layer != LayerMask.NameToLayer("Bullet"))
			{
				component.AddExplosionForce(force, base.transform.position, radius);
			}
			if (component3 != null && component3 != base.transform.GetComponent<ExplodingTarget>())
			{
				component3.TakeDamage((int)(radius - Vector3.Distance(component3.gameObject.transform.position, base.transform.position)));
			}
			if (component2 != null)
			{
				component2.TakeDamage((int)(radius - Vector3.Distance(component2.gameObject.transform.position, base.transform.position)));
			}
		}
		Object.Destroy(base.gameObject, 1f);
	}
}
