using UnityEngine;

public class Ladder : MonoBehaviour
{
	public bool onLadder;

	private void FixedUpdate()
	{
		if (onLadder)
		{
			//Calculating The Force
			Vector3 force = Vector3.up * ((0f - Physics.gravity.y / 3f) * PlayerMovement.Instance.GetRb().mass);
			
			if (PlayerInput.Instance.verticalMovement > 0f)
			{
				force *= 6f;
			}
			
			//Adding Force Up For Going Up The Ladder
			PlayerMovement.Instance.GetRb().AddForce(force);
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			//Changing Players Drag
			PlayerMovement.Instance.GetRb().drag = 6f;
			
			//Setting That We Are On a Ladder
			onLadder = true;
		}
	}

	private void OnCollisionExit(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			//Changing Players Drag
			PlayerMovement.Instance.GetRb().drag = 0f;
			
			//Setting That We Are On Not a Ladder
			onLadder = false;
		}
	}
}
