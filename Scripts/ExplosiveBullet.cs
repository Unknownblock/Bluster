using UnityEngine;

public class ExplosiveBullet : MonoBehaviour
{
	[Header("Amount Variables")]
	public float radius;

	public float force;

	private void FixedUpdate()
	{
		base.transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity, Vector3.up);
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.transform.gameObject.layer != LayerMask.NameToLayer("Player"))
		{
			GameObject obj = Object.Instantiate(PrefabManager.Instance.explosion, base.transform.position, Quaternion.identity);
			obj.GetComponent<Explosion>().radius = radius;
			obj.GetComponent<Explosion>().force = force;
			Object.Destroy(base.gameObject);
		}
	}
}
