using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
	[Header("Health Variables")]
	public int maxHp = 100;

	public int currentHp = 100;
	
	public GameObject healthUI;

	public void Update()
	{
		if (healthUI != null)
		{
			healthUI.SetActive(true);
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
		currentHp -= damageDone; //Damage The Player
		
		DamageIndicator.Instance.target = damageGo.transform; //Setting The Target Of Damage Indicator
		DamageIndicator.Instance.StartFade(); //Starting The Fade Of Damage Indicator

		//Setting Limitations To The Current HP
		if (currentHp > maxHp)
		{
			currentHp = maxHp;
		}
		
		if (currentHp < 0)
		{
			currentHp = 0;
			KillPlayer();
		}
	}

	private void KillPlayer()
	{
		//Setting The Player Details So He Cant Move
		PlayerMovement.Instance.readyToMove = false;
		PlayerMovement.Instance.readyToJump = false;
		PlayerMovement.Instance.readyToCrouch = false;
		
		//Letting The Player Fall Or Rotate
		PlayerMovement.Instance.gameObject.GetComponent<Rigidbody>().freezeRotation = false;
	}
}
