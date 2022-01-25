using TMPro;
using UnityEngine;

public class DamagePopUpColor
{
	public string name;

	public Color32 color;

	public int maxDamage;

	public int minDamage;
}

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
		transform.LookAt(2f * transform.position - playerTransform.position);
		gameObject.transform.position += new Vector3(0f, moveYSpeed * Time.deltaTime, 0f);
	}

	private void Fade()
	{
		disAppearTime -= Time.deltaTime;
		if (disAppearTime <= 0f)
		{
			alpha -= (int)(fadeOutSpeed * Time.deltaTime);
			if (alpha <= 0f)
			{
				Destroy(gameObject);
			}
		}
	}

	private void ColorCheck()
	{
		foreach (DamagePopUpColor popUpColor in damagePopUpColor)
		{
			if (popUpColor.maxDamage <= currentAmount || popUpColor.minDamage >= currentAmount) continue;
			
			textColor.r = popUpColor.color.r;
			textColor.g = popUpColor.color.g;
			textColor.b = popUpColor.color.b;
		}
	}
}
