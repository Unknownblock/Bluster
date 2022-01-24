using UnityEngine;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour
{
	[Header("Size Settings")]
	public float length;

	public float thickness;

	public float gap;

	public float dotSize;

	public bool enableDot;

	[Header("Color")]
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

	private float startGap;

	private void Start()
	{
		startGap = gap;
	}

	public void Update()
	{
		SizeSettings();
		ColorSettings();
		gap = startGap + PlayerMovement.Instance.GetRb().velocity.magnitude;
	}

	private void SizeSettings()
	{
		GetRectTransform(base.transform).sizeDelta = new Vector2(gap, gap);
		GameObject[] array = differentParts;
		for (int i = 0; i < array.Length; i++)
		{
			GetRectTransform(array[i].transform).sizeDelta = new Vector2(thickness, length);
		}
		GetRectTransform(middleDot.transform).sizeDelta = new Vector2(dotSize, dotSize);
		if (enableDot)
		{
			middleDot.SetActive(value: true);
		}
		else if (!enableDot)
		{
			middleDot.SetActive(value: false);
		}
	}

	private void ColorSettings()
	{
		GameObject[] array = differentParts;
		for (int i = 0; i < array.Length; i++)
		{
			GetImageComponent(array[i].transform).color = new Color32(red, green, blue, alpha);
		}
		GetImageComponent(middleDot.transform).color = new Color32(red, green, blue, alpha);
	}

	private static RectTransform GetRectTransform(Transform rectTransform)
	{
		return rectTransform.GetComponent<RectTransform>();
	}

	private static Image GetImageComponent(Component image)
	{
		return image.GetComponent<Image>();
	}
}
