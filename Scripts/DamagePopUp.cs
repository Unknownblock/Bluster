using TMPro;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
	public DamagePopUpColor[] damagePopUpColor;

	public Color32 textColor;

	public float disAppearTime;

	public float fadeOutSpeed;

	public float moveYSpeed;

	public int alpha = 255;

	public int currentAmount;

	public TextMeshPro textMesh;

	public Transform playerTransform;

	public void SetUp(int amount)
	{
		playerTransform = PlayerMovement.Instance.transform;
		textMesh = GetComponent<TextMeshPro>();
		textMesh.SetText(amount.ToString());
		currentAmount = amount;
	}

	private void LateUpdate()
	{
		ColorCheck();
		Fade();
		textColor.a = (byte)alpha;
		textMesh.color = textColor;
		base.transform.LookAt(2f * base.transform.position - playerTransform.position);
		base.gameObject.transform.position += new Vector3(0f, moveYSpeed * Time.deltaTime, 0f);
	}

	private void Fade()
	{
		disAppearTime -= Time.deltaTime;
		if (disAppearTime <= 0f)
		{
			alpha -= (int)(fadeOutSpeed * Time.deltaTime);
			if ((float)alpha <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	private void ColorCheck()
	{
		DamagePopUpColor[] array = this.damagePopUpColor;
		foreach (DamagePopUpColor damagePopUpColor in array)
		{
			if (damagePopUpColor.maxDamage > currentAmount && damagePopUpColor.minDamage < currentAmount)
			{
				textColor.r = damagePopUpColor.color.r;
				textColor.g = damagePopUpColor.color.g;
				textColor.b = damagePopUpColor.color.b;
			}
		}
	}
}
