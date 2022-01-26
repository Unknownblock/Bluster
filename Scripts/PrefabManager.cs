using UnityEngine;

public class PrefabManager : MonoBehaviour
{
	public GameObject explosion;

	public GameObject damagePopUp;

	public static PrefabManager Instance { get; private set; }

	private void Awake()
	{
		//Setting This To a Singleton
		Instance = this;
	}

	public void DisplayDamagePopUp(int amount, Transform popUpParent)
	{
		//Damage Pop Up Instantiating
		Instantiate(damagePopUp, popUpParent.transform.position, Quaternion.identity).GetComponent<DamagePopUp>().SetUp(amount);
	}
}
