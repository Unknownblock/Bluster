using System.Collections;
using TMPro;
using UnityEngine;

public class Shooting : MonoBehaviour
{
	public enum FireMode
	{
		SemiAutomatic,
		FullAutomatic,
		BurstFire
	}

	[Header("Bool Variables")]
	public bool godMode;

	public bool canShoot;

	public bool isShooting;

	public bool isReloading;

	[Header("Shooting Settings")]
	public FireMode fireMode;

	public float fireRate;

	public int currentMagazineAmmo;

	public int currentAmmo;

	public int maxMagazineAmmo;

	public int rbRecoil;

	public int burstAmount;

	public float currentBurstAmount;

	[Header("CoolDown")]
	public float shootCoolDown;

	public float burstDelay;

	[Header("Recoil Settings")]
	[Range(-20f, 20f)]
	public float recoilX;

	[Range(-20f, 20f)]
	public float recoilY;

	[Range(-20f, 20f)]
	public float recoilZ;

	public float snappiness;

	public float returnSpeed;

	[Header("Reload Settings")]
	public int spinAmount;

	public float reloadTime;

	[Header("Bullet Settings")]
	public int bulletsShot;

	public float bulletsSpread;

	public float range;

	public float shootForce;

	public float minDamage;

	public float maxDamage;

	[Header("Assignable")]
	public AudioSource shootSound;

	public AudioSource reloadSound;

	public GameObject projectile;

	public GameObject muzzleFlash;

	public Transform shootPoint;

	public Gun gun;

	[Header("UI Variables")]
	public GameObject gunAmmoCounter;

	public GameObject magazineAmmoCounter;

	public GameObject currentAmmoCounter;

	[Header("Private Variables")]
	private Vector3 currentRotation;
	private Vector3 targetRotation;
	private float nextTimeToFire = 1f;
	
	private void Update()
	{
		//Ammo God Mode
		if (godMode)
		{
			currentMagazineAmmo = maxMagazineAmmo;
			currentAmmo = maxMagazineAmmo;
		}

		WeaponInput();
		
		//Setting Gun Components IsReloading
		gun.isReloading = isReloading;
		
		//Weapon Camera Recoil
		currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
		targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		PlayerInput.Instance.currentRotation = currentRotation;
		
		//Weapon UI Indicators
		if (magazineAmmoCounter != null)
		{
			magazineAmmoCounter.SetActive(true);
			magazineAmmoCounter.GetComponent<TextMeshProUGUI>().SetText(currentMagazineAmmo.ToString());
		}
		
		else if (currentAmmoCounter != null)
		{
			currentAmmoCounter.SetActive(true);
			currentAmmoCounter.GetComponent<TextMeshProUGUI>().SetText(maxMagazineAmmo.ToString());
		}
		
		else if (gunAmmoCounter != null)
		{
			gunAmmoCounter.SetActive(true);
			gunAmmoCounter.GetComponent<TextMeshPro>().SetText(currentMagazineAmmo.ToString());
		}
	}

	private void WeaponInput()
	{
		//Weapon Limitations And CanShoot State
		if (currentMagazineAmmo > maxMagazineAmmo) 
		{
			currentMagazineAmmo = maxMagazineAmmo;
		}
		
		if (currentMagazineAmmo <= maxMagazineAmmo && currentMagazineAmmo > 0)
		{
			canShoot = true;
		}
		
		if (currentMagazineAmmo <= 0) 
		{
			canShoot = false;
			currentMagazineAmmo = 0;
		}

		//Different Firing Modes
		if (fireMode == FireMode.SemiAutomatic)
		{
			if (Input.GetKeyDown(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && currentMagazineAmmo <= maxMagazineAmmo)
			{
				nextTimeToFire = Time.time + 1f / fireRate;
				Shoot();

			}
		}

		if (fireMode == FireMode.FullAutomatic)
		{
			if (Input.GetKey(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && currentMagazineAmmo <= maxMagazineAmmo)
			{
				nextTimeToFire = Time.time + 1f / fireRate;
				Shoot();
			}
		}
		
		if (fireMode == FireMode.BurstFire)
		{
			if (Input.GetKeyDown(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && currentMagazineAmmo <= maxMagazineAmmo)
			{
				Shoot();
				if (canShoot)
				{
					nextTimeToFire = Time.time + 1f / fireRate;
					currentBurstAmount += 1f;
				}

				if (currentBurstAmount >= burstAmount)
				{
					Invoke(nameof(BurstDelay), burstDelay);
					currentBurstAmount = 0f;
				}
			}
		}
		
		else if (PlayerInput.Instance.reloadInput) //Reload If Input Got Pressed
		{
			StartCoroutine(Reload());
		}
	}

	private void RecoilFire()
	{
		//Camera Recoil Adding
		targetRotation += new Vector3(recoilX, Random.Range(0f - recoilY, recoilY), Random.Range(0f - recoilZ, recoilZ));
	}

	private void Shoot()
	{
		if (!canShoot)
		{
			return;
		}
		
		//Shooting For Each Bullet
		for (int i = 0; i < bulletsShot; i++)
		{
			//Instantiating The Bullet
			GameObject bullet = Instantiate(projectile, shootPoint.gameObject.transform.position, Quaternion.identity);

			//Setting The Bullet Spread
			float xSpread = Random.Range(bulletsSpread, -bulletsSpread);
			float ySpread = Random.Range(-bulletsSpread, bulletsSpread);
			float zSpread = Random.Range(bulletsSpread, -bulletsSpread);
			
			//Bullet Damage With Random Value
			if (bullet.GetComponent<Bullet>() != null)
			{
				bullet.GetComponent<Bullet>().damage = (int)Random.Range(minDamage, maxDamage);
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
		
		//Making The Magazine Ammo Less
		currentMagazineAmmo--;
		
		//Shooting Sound
		shootSound.Play();
		
		//Muzzle Flash Instantiation
		GameObject muzzleFlashFx = Instantiate(muzzleFlash, muzzleFlash.transform.position, Quaternion.identity);

		muzzleFlashFx.GetComponent<ParticleSystem>().Play();
		
		//Player KickBack With Shooting
		PlayerMovement.Instance.GetRb().AddForce(-gameObject.transform.forward * rbRecoil, ForceMode.Impulse);
		
		RecoilFire();
		
		//Weapon Recoil Motion
		if (gun != null)
		{
			gun.Shoot();
		}
		
		//Destroying The Muzzle Flash
		Destroy(muzzleFlashFx, 2f);
	}

	private IEnumerator Reload()
	{
		if (currentMagazineAmmo <= maxMagazineAmmo && currentAmmo > 0)
		{
			//Weapon Current State By Reload Being Started
			isReloading = true;
			
			canShoot = false;
			
			//Weapon Reload Motion
			gun.Reload(reloadTime, spinAmount);
			
			//Reload Sound
			reloadSound.Play();
			
			
			yield return new WaitForSeconds(reloadTime);
			
			//Weapon Current State By Reload Being Finished
			isReloading = false;
			
			canShoot = true;
			
			//Making The Current Ammo Less And Calculating The Amount
			if (currentAmmo > maxMagazineAmmo)
			{
				currentAmmo -= maxMagazineAmmo - currentMagazineAmmo;
				currentMagazineAmmo = maxMagazineAmmo;
			}
			
			else if (currentAmmo <= maxMagazineAmmo && currentAmmo > 0)
			{
				currentMagazineAmmo = currentAmmo;
				currentAmmo = 0;
			}
		}
	}

	private void BurstDelay()
	{
		canShoot = true;
	}

	private void IsShooting()
	{
		isShooting = false;
	}
}
