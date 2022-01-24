using TMPro;
using UnityEngine;

public class MoveSpeed : MonoBehaviour
{
	private void Update()
	{
		int num = (int)PlayerMovement.Instance.GetRb().velocity.magnitude;
		base.gameObject.GetComponent<TextMeshProUGUI>().SetText(num + " Km/H");
	}
}
