using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject explosion;

	public GameObject damagePopUp;

	public static PrefabManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void DisplayDamagePopUp(int amount, Transform popUpParent)
	{
		Object.Instantiate(damagePopUp, popUpParent.transform.position, Quaternion.identity).GetComponent<DamagePopUp>().SetUp(amount);
	}
}
