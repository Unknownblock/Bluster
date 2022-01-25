using TMPro;
using UnityEngine;
using System;

[Serializable]
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
		playerTransform = PlayerMovement.Instance.transform; //Set The Player To Look At
		textMesh = GetComponent<TextMeshPro>(); //Set The Text Mesh To Component Of This GameObject
		currentAmount = amount; //Set The Current Amount To The Wanted Amount
		textMesh.SetText(currentAmount.ToString()); //Set The Damage Amount To The Text
	}

	private void LateUpdate()
	{
		ColorCheck();
		Fade();
		textColor.a = (byte) alpha; //Set The Text Colors Opacity Or Alpha
		textMesh.color = textColor; //Set The Color Of Text Mesh To The Wanted Color
		transform.LookAt(2f * transform.position - playerTransform.position); //Look At Player
		gameObject.transform.position += new Vector3(0f, moveYSpeed * Time.deltaTime, 0f); //Damages Pop Up And Moving Up
	}

	private void Fade()
	{
		if (disAppearTime > 0f)
			disAppearTime -= Time.deltaTime; //Fade Out By Time
		
		if (disAppearTime <= 0f)
		{
			alpha -= (int) (fadeOutSpeed * Time.deltaTime); //Color Alpha And Opacity Change
			
			if (alpha <= 0f)
			{
				Destroy(gameObject); //If Color Opacity Equals Zero Destroy
			}
		}
	}

	private void ColorCheck()
	{
		foreach (DamagePopUpColor popUpColor in damagePopUpColor) //Set All The Pop Up Colors By The Max Amount And Min Amount
		{
			if (popUpColor.maxDamage > currentAmount && popUpColor.minDamage < currentAmount)
			{
				textColor.r = popUpColor.color.r;
				textColor.g = popUpColor.color.g;
				textColor.b = popUpColor.color.b;
			}
		}
	}
}
