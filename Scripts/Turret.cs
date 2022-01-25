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

	public Collider[] results;

	public LayerMask targetMask;

	public Vector3 lastKnownPosition = Vector3.zero;

	public Quaternion lookAtRotation;

	[Header("Shooting Settings")]
	public float fireRate;

	public float nextTimeToFire = 1f;

	public float shootCoolDown;

	public bool isShooting;

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

	public GameObject muzzleFlash;

	public GameObject target;

	public GameObject currentTarget;

	public void Update()
	{
		//Rotating Towards The Target
		if (currentTarget != null)
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
		
		//Detecting The Target By The Area Around The Turret
		if (detectionMode == DetectionMode.AreaDetection)
		{
			//Getting The Object And Colliders In The Radius
			// ReSharper disable once Unity.PreferNonAllocApi
			results = Physics.OverlapSphere(gameObject.transform.position, radius, targetMask);

			foreach (Collider everyResult in results) //Selecting The Object To Look And Shoot At
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

		//One Target Detection
		else if (detectionMode == DetectionMode.OneObjectDetection && target != null)
		{
			//Calculating The Distance From The Turret
			float distanceFromTarget = Vector3.Distance(target.transform.position, transform.position);
			
			if (distanceFromTarget <= radius) //Setting The Object To Our CurrentTarget If In Radius
			{
				SetTarget(target.gameObject);
				WeaponInput();
			}
			
			else if (distanceFromTarget > radius) //Setting It To Null
			{
				SetTarget(null);
			}
		}
	}

	private void WeaponInput()
	{
		//Shoot When The Target Is In Front Of The Turret
		if (Time.time >= nextTimeToFire && Physics.Raycast(shootPoint.transform.position, shootPoint.forward, radius, targetMask))
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot();
		}
	}

	private void Shoot()
	{
		for (int i = 0; i < bulletsShot; i++)
		{
			//Instantiating The Bullet
			GameObject bullet = Instantiate(projectile, shootPoint.gameObject.transform.position, Quaternion.identity);
			
			//Setting The Bullet Spread
			float xSpread = Random.Range(bulletsSpread, -bulletsSpread);
			float ySpread = Random.Range(-bulletsSpread, bulletsSpread);
			float zSpread = Random.Range(bulletsSpread, -bulletsSpread);
			
			//Bullet Damage With Random Value
			if (bullet.GetComponent<EnemyBullet>() != null)
			{
				bullet.GetComponent<EnemyBullet>().damage = (int)Random.Range(minDamage, maxDamage);
				bullet.GetComponent<EnemyBullet>().SetShooter(transform.gameObject);
			}
			
			//Rotating The Bullet By The Spread
			bullet.transform.forward = shootPoint.forward + new Vector3(xSpread, ySpread, zSpread);
			
			//Adding Force To The Bullet By The Shoot Force
			bullet.GetComponent<Rigidbody>().AddForce(shootForce * bullet.transform.forward, ForceMode.VelocityChange);
			
			//Adding Range To The Bullets
			Destroy(bullet, range);
		}
		
		//Is Shooting Bool With CoolDown
		isShooting = true;
		Invoke(nameof(IsShooting), shootCoolDown);
		
		//Playing The Shooting Sound
		shootSound.Play();
		
		//Muzzle Flash Instantiation
		GameObject muzzleFlashFx = Instantiate(muzzleFlash, muzzleFlash.transform.position, Quaternion.identity);
		muzzleFlashFx.GetComponent<ParticleSystem>().Play();
		
		//Destroying The Muzzle Flash
		Destroy(muzzleFlashFx, 2f);
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
