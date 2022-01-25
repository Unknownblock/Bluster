using System.Globalization;
using TMPro;
using UnityEngine;

public class AimPracticeTarget : MonoBehaviour
{
	public bool resetEveryShoot;

	public float points;

	public float pointForEveryShot;

	public GameObject pointUI;

	public GameObject middleHitPoint;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.T)) //Reset The Points
		{
			points = 0f;
		}
		
		if (pointUI != null) //Setting The Point UI To Our Points
		{
			pointUI.GetComponent<TextMeshProUGUI>().SetText(points.ToString(CultureInfo.CurrentCulture));
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Bullet")) //See If a Bullet Hit Our Target
		{
			if (resetEveryShoot) //Resetting The UI When Needed
			{
				points = 0f;
			}
			
			points += pointForEveryShot - (int)(4f * Vector3.Distance(middleHitPoint.transform.position, other.contacts[0].point)); // Adding The Point Wanted
		}
	}
}
