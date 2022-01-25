using UnityEngine;

public class Turret : MonoBehaviour
{
	public enum DetectionMode
	{
		AreaDetection,
		OneObjectDetection
	}
	
	[Header("Turret Settings")]
	
	public DetectionMode detectionMode;

	public float speed = 3f;

	public float radius;

	public LayerMask targetMask;

	private Vector3 lastKnownPosition = Vector3.zero;

	private Quaternion lookAtRotation;

	[Header("Shooting Settings")]
	public float fireRate;

	public float nextTimeToFire = 1f;

	public float shootCoolDown;

	public bool isShooting;

	private float alpha;

	[Header("Bullet Settings")]
	public int bulletsShot;

	public float bulletsSpread;

	public float range;

	public float shootForce;

	public float minDamage;

	public float maxDamage;

	[Header("Assignable")]
	public Transform shootPoint;

	public AudioSource shootSound;

	public GameObject projectile;

	public GameObject smokeTrail;

	public GameObject muzzleFlash;

	public GameObject target;

	public GameObject currentTarget;

	private int amount;

	public void Update()
	{
		alpha -= Time.deltaTime * 1f;
		if (isShooting)
		{
			alpha = 1f;
		}
		smokeTrail.GetComponent<TrailRenderer>().startColor = new Color(1f, 1f, 1f, alpha);
		if ((bool)currentTarget)
		{
			if (lastKnownPosition != currentTarget.transform.position)
			{
				lastKnownPosition = currentTarget.transform.position;
				lookAtRotation = Quaternion.LookRotation(lastKnownPosition - transform.position);
			}
			if (transform.rotation != lookAtRotation)
			{
				transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtRotation, speed * Time.deltaTime);
			}
		}
		if (detectionMode == DetectionMode.AreaDetection)
		{
			Collider[] results = Physics.OverlapSphere(gameObject.transform.position, radius, targetMask);
			
			foreach (Collider everyResult in results)
			{
				if (results.Length > 0)
				{
					WeaponInput();
					SetTarget(everyResult.gameObject);
				}
				
				else if (results.Length == 0)
				{
					SetTarget(null);
				}
			}
		}
		
		else if (detectionMode == DetectionMode.OneObjectDetection && target != null)
		{
			float num = Vector3.Distance(target.transform.position, transform.position);
			
			if (num <= radius)
			{
				SetTarget(target.gameObject);
				WeaponInput();
			}
			else if (num > radius)
			{
				SetTarget(null);
			}
		}
	}

	private void WeaponInput()
	{
		if (Time.time >= nextTimeToFire && Physics.Raycast(shootPoint.transform.position, shootPoint.forward, out var hitInfo, radius) && hitInfo.transform.gameObject == currentTarget)
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot();
		}
	}

	private void Shoot()
	{
		for (int i = 0; i < bulletsShot; i++)
		{
			GameObject gameObject = Instantiate(projectile, shootPoint.gameObject.transform.position, Quaternion.identity);
			float x = Random.Range(bulletsSpread, 0f - bulletsSpread);
			float y = Random.Range(0f - bulletsSpread, bulletsSpread);
			float z = Random.Range(bulletsSpread, 0f - bulletsSpread);
			if (gameObject.GetComponent<EnemyBullet>() != null)
			{
				gameObject.GetComponent<EnemyBullet>().damage = (int)Random.Range(minDamage, maxDamage);
				gameObject.GetComponent<EnemyBullet>().SetShooter(transform.gameObject);
			}
			gameObject.transform.forward = shootPoint.forward + new Vector3(x, y, z);
			gameObject.GetComponent<Rigidbody>().AddForce(shootForce * gameObject.transform.forward, ForceMode.VelocityChange);
			Destroy(gameObject, range);
		}
		shootSound.Play();
		isShooting = true;
		Invoke("IsShooting", shootCoolDown);
		GameObject obj = Instantiate(muzzleFlash, muzzleFlash.transform.position, Quaternion.identity);
		smokeTrail.GetComponent<TrailRenderer>().colorGradient.alphaKeys[0].alpha = 1f;
		obj.GetComponent<ParticleSystem>().Play();
		Destroy(obj, 2f);
	}

	private void SetTarget(GameObject wantedTarget)
	{
		currentTarget = wantedTarget;
	}

	private void IsShooting()
	{
		isShooting = false;
	}
}
