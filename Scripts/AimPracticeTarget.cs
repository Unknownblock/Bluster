using TMPro;
using UnityEngine;

public class AimPracticeTarget : MonoBehaviour
{
	public bool resetEveryShoot;

	public float points;

	public GameObject pointUI;

	public GameObject middleHitPoint;

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.T))
		{
			points = 0f;
		}
		if (pointUI != null)
		{
			pointUI.GetComponent<TextMeshProUGUI>().SetText(points.ToString());
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Bullet"))
		{
			if (resetEveryShoot)
			{
				points = 0f;
			}
			points += 20f - (float)(int)(4f * Vector3.Distance(middleHitPoint.transform.position, other.contacts[0].point));
		}
	}
}
