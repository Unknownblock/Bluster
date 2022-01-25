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

	private float _startGap;

	private void Start()
	{
		_startGap = gap;
	}

	public void Update()
	{
		SizeSettings();
		ColorSettings();
		gap = _startGap + PlayerMovement.Instance.GetRb().velocity.magnitude;
	}

	private void SizeSettings()
	{
		GetRectTransform(transform).sizeDelta = new Vector2(gap, gap);

		foreach (var everyObject in differentParts)
		{
			GetRectTransform(everyObject.transform).sizeDelta = new Vector2(thickness, length);
		}
		
		GetRectTransform(middleDot.transform).sizeDelta = new Vector2(dotSize, dotSize);
		
		if (enableDot)
		{
			middleDot.SetActive(true);
		}
		else if (!enableDot)
		{
			middleDot.SetActive(false);
		}
	}

	private void ColorSettings()
	{
		foreach (var everyObject in differentParts)
		{
			GetImageComponent(everyObject.transform).color = new Color32(red, green, blue, alpha);
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
