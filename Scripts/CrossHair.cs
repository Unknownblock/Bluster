using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CrossHairPart
{
	public Transform transform;
	
	public enum PartType
	{ Dot, NormalPart, InvertedSizePart }
	
	public PartType type;
}

public class CrossHair : MonoBehaviour
{
	[Header("CrossHair Size Settings")]
	public float length;

	public float thickness;

	public float gap;

	public float dotSize;

	public bool enableDot;
	
	[Header("CrossHair Color")]
	[Range(0f, 255f)]
	public byte red;

	[Range(0f, 255f)]
	public byte green;

	[Range(0f, 255f)]
	public byte blue;

	[Range(0f, 255f)]
	public byte alpha;
	
	[Header("Outline Size Settings")]
	public float outlineAmount;

	[Header("Assignable")]
	public CrossHairPart[] differentParts;
	public CrossHairPart[] outlineParts;
	
	public void Update()
	{
		SizeSettings();
		ColorSettings();
		OutlineSizeSettings();
	}

	private void SizeSettings()
	{
		GetRectTransform(transform).sizeDelta = new Vector2(gap, gap); //Setting The Gap

		foreach (var everyObject in differentParts)
		{
			if (everyObject.type == CrossHairPart.PartType.NormalPart)
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(thickness, length); //Setting The Length And Thickness Of All The CrossHair Parts
			
			if (everyObject.type == CrossHairPart.PartType.InvertedSizePart)
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(length, thickness); //Setting The Length And Thickness Of All The CrossHair Parts

			if (everyObject.type == CrossHairPart.PartType.Dot)
			{
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(dotSize, dotSize); //Setting The Middle Dots Size

				if (enableDot)
				{
					everyObject.transform.gameObject.SetActive(true); //Turning The Dot On
				}

				else if (!enableDot)
				{
					everyObject.transform.gameObject.SetActive(false); //Turning The Dot Off
				}
			}
		}
	}

	private void OutlineSizeSettings()
	{
		GetRectTransform(transform).sizeDelta = new Vector2(gap, gap); //Setting The Gap

		foreach (var everyObject in outlineParts)
		{
			var outlineLength = length + outlineAmount;
			var outlineThickness = thickness + outlineAmount;
			var outlineDotSize = dotSize + outlineAmount;

			if (everyObject.type == CrossHairPart.PartType.NormalPart)
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(outlineThickness, outlineLength);
			
			if (everyObject.type == CrossHairPart.PartType.InvertedSizePart)
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(outlineLength, outlineThickness);
			
			if (everyObject.type == CrossHairPart.PartType.Dot)
			{
				GetRectTransform(everyObject.transform).sizeDelta = new Vector2(outlineDotSize, outlineDotSize);
				
				if (enableDot)
				{
					everyObject.transform.gameObject.SetActive(true); //Turning The Dot On
				}

				else if (!enableDot)
				{
					everyObject.transform.gameObject.SetActive(false); //Turning The Dot Off
				}
			}
		}
	}

	private void ColorSettings()
	{
		foreach (var everyObject in differentParts)
		{
			GetImageComponent(everyObject.transform).color = new Color32(red, green, blue, alpha); //Set The Color Of All The Parts Of The CrossHair
		}
	}

	private static RectTransform GetRectTransform(Transform rectTransform)
	{
		return rectTransform.GetComponent<RectTransform>(); //Get The RectTransform Component
	}

	private static Image GetImageComponent(Component image)
	{
		return image.GetComponent<Image>(); //Get The Image Component
	}
}
