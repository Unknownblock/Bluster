using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
	public float fadeOutSpeed;

	public Transform arrow;

	public Transform target;

	public Vector3 pointing;

	private Vector3 _startScale;

	public static DamageIndicator Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
		_startScale = transform.GetChild(0).localScale;
	}

	private void Update()
	{
		Arrows();
		transform.GetChild(0).localScale = Vector3.Lerp(transform.GetChild(0).localScale, Vector3.zero, Time.deltaTime * fadeOutSpeed);
	}

	public void StartFade()
	{
		transform.GetChild(0).localScale = _startScale;
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
