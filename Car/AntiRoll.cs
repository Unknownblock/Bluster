// AntiRoll
using UnityEngine;

public class AntiRoll : MonoBehaviour
{
	public Suspension right;

	public Suspension left;

	public float antiRoll = 5000f;

	private Rigidbody _rb;

	private void Awake()
	{
		_rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		StabilizerBars();
	}

	private void StabilizerBars()
	{
		var num2 = !right.isGrounded ? 1f : right.lastCompression;
		var num = !left.isGrounded ? 1f : left.lastCompression;
		
		float num3 = (num - num2) * antiRoll;
		
		if (right.isGrounded)
		{
			_rb.AddForceAtPosition(right.transform.up * (0f - num3), right.gameObject.transform.position);
		}
		
		if (left.isGrounded)
		{
			_rb.AddForceAtPosition(left.transform.up * num3, left.gameObject.transform.position);
		}
	}
}
