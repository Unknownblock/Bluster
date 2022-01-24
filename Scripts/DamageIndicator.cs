using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
	public float fadeOutSpeed;

	public Transform arrow;

	public Transform target;

	public Vector3 pointing;

	private Vector3 startScale;

	public static DamageIndicator Instance { get; set; }

	private void Awake()
	{
		Instance = this;
		startScale = base.transform.GetChild(0).localScale;
	}

	private void Update()
	{
		Arrows();
		base.transform.GetChild(0).localScale = Vector3.Lerp(base.transform.GetChild(0).localScale, Vector3.zero, Time.deltaTime * fadeOutSpeed);
	}

	public void StartFade()
	{
		base.transform.GetChild(0).localScale = startScale;
	}

	private void Arrows()
	{
		if (Camera.main != null)
		{
			Vector3 vector = Camera.main.WorldToScreenPoint(target.transform.position);
			pointing.z = Mathf.Atan2(arrow.transform.position.y - vector.y, arrow.gameObject.transform.position.x - vector.x) * 57.29578f - 90f;
		}
		arrow.transform.rotation = Quaternion.Euler(pointing);
	}
}
