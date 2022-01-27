using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
	[Header("CrossHair Size Settings")]
	public float length;

	public float thickness;

	public float gap;

	public float dotSize;

	public bool enableDot;

	[Header("HitMaker Size Settings")] public float hitMarkerLength;
	public float hitMarkerThickness;
	

	[Header("CrossHair Color")]
	[Range(0f, 255f)]
	public byte red;

	[Range(0f, 255f)]
	public byte green;

	[Range(0f, 255f)]
	public byte blue;

	[Range(0f, 255f)]
	public byte alpha;

	[Header("Assignable")]
	public GameObject[] differentParts;

	public GameObject middleDot;
	
	public void Update()
	{
		SizeSettings();
		ColorSettings();
	}

	private void SizeSettings()
	{
		GetRectTransform(transform).sizeDelta = new Vector2(gap, gap); //Setting The Gap

		foreach (var everyObject in differentParts)
		{
			GetRectTransform(everyObject.transform).sizeDelta = new Vector2(thickness, length); //Setting The Length And Thickness Of All The CrossHair Parts
		}
		
		GetRectTransform(middleDot.transform).sizeDelta = new Vector2(dotSize, dotSize); //Setting The Middle Dots Size
		
		if (enableDot)
		{
			middleDot.SetActive(true); //Turning The Dot On
		}
		else if (!enableDot)
		{
			middleDot.SetActive(false); //Turning The Dot Off
		}
	}

	private void ColorSettings()
	{
		foreach (var everyObject in differentParts)
		{
			GetImageComponent(everyObject.transform).color = new Color32(red, green, blue, alpha); //Set The Color Of All The Parts Of The CrossHair
		}

		GetImageComponent(middleDot.transform).color = new Color32(red, green, blue, alpha); //Set The Color Of The Middle Part
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
