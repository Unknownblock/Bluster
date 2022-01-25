using UnityEngine;

public class Gun : MonoBehaviour
{
	[Header("Gun Main")]
	public float gunGetBackSmooth = 25f;

	private float gunPosOffSet;

	[Header("Gun Speed Bob")]
	public float speedBobMultiplier = 1f;

	public float speedBobSmooth = 10f;

	private Vector3 speedBob;

	[Header("Gun Movement Bob")]
	public float xBob = 0.12f;

	public float yBob = 0.08f;

	public float zBob = 0.1f;

	public float bobSpeed = 0.5f;

	public bool isBobbing;

	private Vector3 desiredBob;

	[Header("Gun Recoil OffSet")]
	public float forwardRecoil = 1f;

	public float rightRecoil = 0.3f;

	public float upRecoil = 0.3f;

	[Header("Gun Recoil Rotation")]
	public float recoilRotationAmount = 1.5f;

	public float xRecoil = 90f;

	public float yRecoilRandomize = 20f;

	public float zRecoilRandomize = 50f;

	[Header("Gun Reload")]
	public ReloadDirection reloadDirection;

	public bool isReloading;

	public float reloadOffSet = 0.45f;

	public float reloadProgress;

	private float reloadRotation;

	private float desiredReloadRotation;

	private float reloadTime;

	private int spinsAmount;

	[Header("Gun Sway")]
	public float gunDrag = 0.2f;

	public float swaySpeed = 10f;

	private float desX;

	private float desY;

	[Header("Others")]
	private Vector3 startPos;

	private Vector3 recoilOffset;

	private Vector3 recoilRotation;

	private void Start()
	{
		startPos = transform.localPosition;
	}

	private void Update()
	{
		MovementBob();
		ReloadGun();
		RecoilGun();
		SpeedBob();
		float b = (0f - Input.GetAxis("Mouse X")) * gunDrag;
		float b2 = (0f - Input.GetAxis("Mouse Y")) * gunDrag;
		desX = Mathf.Lerp(desX, b, Time.fixedDeltaTime * swaySpeed);
		desY = Mathf.Lerp(desY, b2, Time.fixedDeltaTime * swaySpeed);
		Rotation(new Vector2(desX, desY));
		transform.localPosition = Vector3.Lerp(transform.localPosition, startPos + new Vector3(desX, desY, 0f) + desiredBob + recoilOffset + new Vector3(0f, 0f - gunPosOffSet, 0f) + speedBob, 15f * Time.fixedDeltaTime);
	}

	private void Rotation(Vector2 offset)
	{
		float num = offset.magnitude * 0.03f;
		if (offset.x < 0.0)
		{
			num = 0f - num;
		}
		Vector3 euler = new Vector3((float)(offset.y * 80.0) + reloadRotation, (0f - offset.x) * 40f, num * 50f) + recoilRotation;
		transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(euler), Time.deltaTime * 25f);
	}

	private void MovementBob()
	{
		if (PlayerMovement.Instance.GetRb().velocity.magnitude > 4.0 && PlayerMovement.Instance.isGrounded && !PlayerMovement.Instance.isSliding)
		{
			desiredBob = new Vector3(Mathf.PingPong(Time.time * bobSpeed, xBob) - xBob / 2f, Mathf.PingPong(Time.time * bobSpeed, yBob) - yBob / 2f, Mathf.PingPong(Time.time * bobSpeed, zBob) - zBob / 2f);
			isBobbing = true;
		}
		else
		{
			isBobbing = false;
		}
		if (!isBobbing)
		{
			desiredBob = Vector3.Lerp(desiredBob, Vector3.zero, Time.deltaTime * gunGetBackSmooth);
		}
	}

	private void SpeedBob()
	{
		Vector2 vector = PlayerMovement.Instance.FindVelRelativeToLook();
		speedBob = Vector3.Lerp(speedBob, Vector3.ClampMagnitude(new Vector3(vector.x, PlayerMovement.Instance.GetVelocity().y, vector.y) * speedBobMultiplier * -0.01f, 1f), Time.deltaTime * speedBobSmooth);
	}

	private void RecoilGun()
	{
		recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * gunGetBackSmooth);
		recoilRotation = Vector3.Lerp(recoilRotation, Vector3.zero, Time.deltaTime * gunGetBackSmooth);
	}

	private void ReloadGun()
	{
		if (isReloading)
		{
			reloadProgress += Time.deltaTime / reloadTime;
			reloadRotation = Mathf.Lerp(0f, desiredReloadRotation, reloadProgress * reloadTime / reloadTime);
			gunPosOffSet = Mathf.Lerp(gunPosOffSet, 0f, Time.deltaTime * reloadTime * 2f);
		}
		else
		{
			reloadProgress = 0f;
			reloadRotation = 0f;
		}
		if (reloadRotation / 360f > spinsAmount)
		{
			spinsAmount++;
		}
	}

	public void Shoot()
	{
		recoilOffset += -(Vector3.forward * forwardRecoil - Vector3.down * upRecoil - Vector3.right * rightRecoil);
		recoilRotation += -new Vector3(xRecoil, Random.Range(yRecoilRandomize, yRecoilRandomize), Random.Range(zRecoilRandomize, zRecoilRandomize)) * recoilRotationAmount;
	}

	public void Reload(float reloadingTime, int spinAmount)
	{
		reloadTime = reloadingTime;
		spinsAmount = 0;
		if (spinAmount < 1)
		{
			spinAmount = Mathf.RoundToInt(reloadingTime * 3f);
		}
		if (reloadDirection == ReloadDirection.Backward)
		{
			desiredReloadRotation = -360 * spinAmount;
		}
		else if (reloadDirection == ReloadDirection.Forward)
		{
			desiredReloadRotation = 360 * spinAmount;
		}
		else if (reloadDirection == ReloadDirection.None)
		{
			desiredReloadRotation = 0f;
		}
		gunPosOffSet = reloadOffSet;
	}
}
