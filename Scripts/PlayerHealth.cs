using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[Header("Health Variables")]
	public int maxHp = 100;

	public int currentHp = 100;

	[Header("UI Variables")]
	public GameObject deadUI;

	public GameObject normalUI;

	public GameObject healthUI;

	public void Update()
	{
		if (healthUI != null)
		{
			healthUI.SetActive(value: true);
			healthUI.GetComponent<TextMeshProUGUI>().SetText(currentHp.ToString());
		}
		if (currentHp < 0)
		{
			currentHp = 0;
			KillPlayer();
		}
	}

	public void Damage(int damageDone, GameObject damageGo)
	{
		currentHp -= damageDone;
		DamageIndicator.Instance.target = damageGo.transform;
		DamageIndicator.Instance.StartFade();
		if (currentHp < 0)
		{
			currentHp = 0;
			KillPlayer();
		}
	}

	private void KillPlayer()
	{
		PlayerMovement.Instance.readyToMove = false;
		PlayerMovement.Instance.readyToJump = false;
		PlayerMovement.Instance.readyToCrouch = false;
		deadUI.SetActive(value: true);
		normalUI.SetActive(value: false);
		PlayerMovement.Instance.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
	}
}
