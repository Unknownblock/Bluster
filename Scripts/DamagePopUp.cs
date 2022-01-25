using TMPro;
using UnityEngine;

public abstract class DamagePopUpColor
{
	public Color32 Color;

	public readonly int MaxDamage;

	public readonly int MinDamage;

	protected DamagePopUpColor(Color32 color, int maxDamage, int minDamage)
	{
		Color = color;
		MaxDamage = maxDamage;
		MinDamage = minDamage;
	}
}

public class DamagePopUp : MonoBehaviour
{
	private readonly DamagePopUpColor[] _damagePopUpColor;

	public Color32 textColor;

	public float disAppearTime;

	public float fadeOutSpeed;

	public float moveYSpeed;

	public int alpha = 255;

	public int currentAmount;

	public TextMeshPro textMesh;

	public Transform playerTransform;

	public DamagePopUp(DamagePopUpColor[] damagePopUpColor)
	{
		_damagePopUpColor = damagePopUpColor;
	}

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
		foreach (DamagePopUpColor popUpColor in _damagePopUpColor)
		{
			if (popUpColor.MaxDamage <= currentAmount || popUpColor.MinDamage >= currentAmount) continue;
			
			textColor.r = popUpColor.Color.r;
			textColor.g = popUpColor.Color.g;
			textColor.b = popUpColor.Color.b;
		}
	}
}
