using System.Collections;
using TMPro;
using UnityEngine;

public class Shooting : MonoBehaviour
{
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

	public GameObject smokeTrail;

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

	private float alpha;

	private void Start()
	{
		if (currentMagazineAmmo == -1)
		{
			currentMagazineAmmo = maxMagazineAmmo;
		}
	}

	private void Update()
	{
		alpha -= Time.deltaTime * 1f;
		if (isShooting)
		{
			alpha = 1f;
		}
		smokeTrail.GetComponent<TrailRenderer>().startColor = new Color(1f, 1f, 1f, alpha);
		if (godMode)
		{
			maxMagazineAmmo = int.MaxValue;
			currentMagazineAmmo = int.MaxValue;
			currentAmmo = int.MaxValue;
		}
		WeaponInput();
		StartCoroutine(IsShooting());
		gun.isReloading = isReloading;
		PlayerInput.Instance.currentRotation = currentRotation;
		targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
		currentRotation = Vector3.Slerp(currentRotation, targetRotation, snappiness * Time.fixedDeltaTime);
		if (magazineAmmoCounter != null)
		{
			magazineAmmoCounter.SetActive(value: true);
			magazineAmmoCounter.GetComponent<TextMeshProUGUI>().SetText(currentMagazineAmmo.ToString());
		}
		else if (currentAmmoCounter != null)
		{
			currentAmmoCounter.SetActive(value: true);
			currentAmmoCounter.GetComponent<TextMeshProUGUI>().SetText(maxMagazineAmmo.ToString());
		}
		else if (gunAmmoCounter != null)
		{
			gunAmmoCounter.SetActive(value: true);
			gunAmmoCounter.GetComponent<TextMeshPro>().SetText(currentMagazineAmmo.ToString());
		}
	}

	private void WeaponInput()
	{
		if (currentMagazineAmmo > maxMagazineAmmo)
		{
			currentMagazineAmmo = maxMagazineAmmo;
		}
		if (currentMagazineAmmo <= 0)
		{
			canShoot = false;
		}

		if (Input.GetKeyDown(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && fireMode == FireMode.SemiAutomatic &&
		    currentMagazineAmmo <= maxMagazineAmmo)
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot();

		}

		if (Input.GetKey(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && fireMode == FireMode.FullAutomatic && currentMagazineAmmo <= maxMagazineAmmo)
		{
			nextTimeToFire = Time.time + 1f / fireRate;
			Shoot();
		}
		
		if (Input.GetKeyDown(InputManager.Instance.shootKey) && Time.time >= nextTimeToFire && fireMode == FireMode.BurstFire && currentMagazineAmmo <= maxMagazineAmmo)
		{
			Shoot();
			if (canShoot)
			{
				nextTimeToFire = Time.time + 1f / fireRate;
				currentBurstAmount += 1f;
			}
			if (currentBurstAmount >= burstAmount)
			{
				StartCoroutine(BurstWait());
				currentBurstAmount = 0f;
			}
		}
		if (isReloading)
		{
			canShoot = false;
		}
		else if (currentMagazineAmmo <= 0 || PlayerInput.Instance.reloadInput)
		{
			StartCoroutine(Reload());
		}
	}

	private void RecoilFire()
	{
		targetRotation += new Vector3(recoilX, Random.Range(0f - recoilY, recoilY), Random.Range(0f - recoilZ, recoilZ));
	}

	private void Shoot()
	{
		if (!canShoot)
		{
			return;
		}
		
		for (int i = 0; i < bulletsShot; i++)
		{
			GameObject bullet = Instantiate(projectile, shootPoint.gameObject.transform.position, Quaternion.identity);
			
			float xSpread = Random.Range(bulletsSpread, -bulletsSpread);
			float ySpread = Random.Range(-bulletsSpread, bulletsSpread);
			float zSpread = Random.Range(bulletsSpread, -bulletsSpread);
			
			if (bullet.GetComponent<Bullet>() != null)
			{
				bullet.GetComponent<Bullet>().damage = (int)Random.Range(minDamage, maxDamage);
			}
			
			bullet.transform.forward = shootPoint.forward + new Vector3(xSpread, ySpread, zSpread);
			
			bullet.GetComponent<Rigidbody>().AddForce(shootForce * bullet.transform.forward, ForceMode.VelocityChange);
			Destroy(bullet, range);
		}
		
		currentMagazineAmmo--;
		
		shootSound.Play();
		
		GameObject obj = Instantiate(muzzleFlash, muzzleFlash.transform.position, Quaternion.identity);
		
		smokeTrail.GetComponent<TrailRenderer>().colorGradient.alphaKeys[0].alpha = 1f;
		
		obj.GetComponent<ParticleSystem>().Play();
		
		PlayerMovement.Instance.GetRb().AddForce(-this.gameObject.transform.forward * rbRecoil, ForceMode.Impulse);
		
		RecoilFire();
		
		if (gun != null)
		{
			gun.Shoot();
		}
		
		Destroy(obj, 2f);
	}

	private IEnumerator Reload()
	{
		if (currentMagazineAmmo < maxMagazineAmmo && currentAmmo > 0)
		{
			isReloading = true;
			canShoot = false;
			gun.Reload(reloadTime, spinAmount);
			reloadSound.Play();
			yield return new WaitForSeconds(reloadTime);
			isReloading = false;
			canShoot = true;
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

	private IEnumerator BurstWait()
	{
		canShoot = false;
		yield return new WaitForSeconds(burstDelay);
		canShoot = true;
	}

	private IEnumerator IsShooting()
	{
		if (Input.GetButtonDown("Fire2") && canShoot)
		{
			isShooting = true;
		}
		if (Input.GetButtonUp("Fire2"))
		{
			yield return new WaitForSeconds(shootCoolDown);
			isShooting = false;
		}
	}
}
