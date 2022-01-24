using UnityEngine;
using UnityEngine.UI;

public class WeaponContainer : MonoBehaviour
{
	public bool havePrimaryWeapon;

	public bool haveSecondaryWeapon;

	public bool isPrimaryWeapon;

	public bool isSecondaryWeapon;

	public GameObject shootPoint;

	public GameObject crossHair;

	public GameObject reloadSprite;

	public GameObject primaryWeapon;

	public GameObject secondaryWeapon;

	public void Update()
	{
		primaryWeapon = base.gameObject.transform.GetChild(0).gameObject;
		secondaryWeapon = base.gameObject.transform.GetChild(1).gameObject;
		if (primaryWeapon != null && havePrimaryWeapon)
		{
			primaryWeapon = base.gameObject.transform.GetChild(0).gameObject;
			if (primaryWeapon.GetComponent<Gun>().isReloading)
			{
				crossHair.SetActive(value: false);
				reloadSprite.SetActive(value: true);
				if (isPrimaryWeapon)
				{
					reloadSprite.gameObject.GetComponent<Image>().fillAmount = primaryWeapon.GetComponent<Gun>().reloadProgress;
				}
				if (isSecondaryWeapon)
				{
					reloadSprite.gameObject.GetComponent<Image>().fillAmount = secondaryWeapon.GetComponent<Gun>().reloadProgress;
				}
			}
			if (primaryWeapon.activeInHierarchy)
			{
				isPrimaryWeapon = true;
			}
			if (reloadSprite.GetComponent<Image>().fillAmount >= 1f)
			{
				crossHair.SetActive(value: true);
				reloadSprite.SetActive(value: false);
			}
			else
			{
				crossHair.SetActive(value: false);
				reloadSprite.SetActive(value: true);
			}
		}
		if (secondaryWeapon != null)
		{
			if (haveSecondaryWeapon && secondaryWeapon != shootPoint && secondaryWeapon.GetComponent<Gun>() != null && secondaryWeapon.GetComponent<Gun>().isReloading)
			{
				crossHair.SetActive(value: false);
				reloadSprite.SetActive(value: true);
				if (isPrimaryWeapon)
				{
					reloadSprite.gameObject.GetComponent<Image>().fillAmount = primaryWeapon.GetComponent<Gun>().reloadProgress;
				}
				if (isSecondaryWeapon)
				{
					reloadSprite.gameObject.GetComponent<Image>().fillAmount = secondaryWeapon.GetComponent<Gun>().reloadProgress;
				}
			}
			if (secondaryWeapon.activeInHierarchy)
			{
				isSecondaryWeapon = true;
			}
			if (reloadSprite.GetComponent<Image>().fillAmount >= 1f)
			{
				crossHair.SetActive(value: true);
				reloadSprite.SetActive(value: false);
			}
			else
			{
				crossHair.SetActive(value: false);
				reloadSprite.SetActive(value: true);
			}
		}
		WeaponSwitching();
	}

	private void WeaponSwitching()
	{
		if (havePrimaryWeapon && Input.GetButtonDown("PrimaryWeapon"))
		{
			primaryWeapon.SetActive(value: true);
			secondaryWeapon.SetActive(value: false);
			isPrimaryWeapon = true;
			isSecondaryWeapon = false;
		}
		if (haveSecondaryWeapon && Input.GetButtonDown("SeconderyWeapon"))
		{
			primaryWeapon.SetActive(value: false);
			secondaryWeapon.SetActive(value: true);
			isPrimaryWeapon = false;
			isSecondaryWeapon = true;
		}
	}
}
