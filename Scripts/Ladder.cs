using UnityEngine;

public class Ladder : MonoBehaviour
{
	private bool onLadder;

	private void FixedUpdate()
	{
		if (onLadder)
		{
			Vector3 force = Vector3.up * ((0f - Physics.gravity.y / 3f) * PlayerMovement.Instance.GetRb().mass);
			if (PlayerInput.Instance.verticalMovement > 0f)
			{
				force *= 6f;
			}
			PlayerMovement.Instance.GetRb().AddForce(force);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			PlayerMovement.Instance.GetRb().drag = 6f;
			onLadder = true;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			PlayerMovement.Instance.GetRb().drag = 0f;
			onLadder = false;
		}
	}
}
