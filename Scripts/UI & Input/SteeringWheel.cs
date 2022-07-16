using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class SteeringWheel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	public bool isPressed;
	
	[Header("Steering Wheel Max Angle")]
	public float maxAngle = 450f;

	[Header("Degrees Per Second")]
	public float maxReleaseSpeed = 350f;
	public float releaseSpeed;
	public float currentReleaseSpeed;

	public float steeringInput;
	
	public float wheelAngle;
	public float wheelPrevAngle;

	public Vector2 centerPoint;


	private RectTransform _wheel;

	private void Start()
	{
		_wheel = GetComponent<RectTransform>();
	}

	private void FixedUpdate()
	{
		releaseSpeed = maxReleaseSpeed * currentReleaseSpeed;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		StartCalculatingWheelRotation(eventData);
		
		isPressed = true;
	}
	
	public void OnPointerUp(PointerEventData eventData)
	{
		OnDrag(eventData);

		StartCoroutine(ReleaseWheel());
		
		isPressed = false;
	}

	public void OnDrag(PointerEventData eventData)
	{
		CalculateWheelRotation(eventData);

		UpdateWheelImage();

		CalculateInput();
	}

	private void StartCalculatingWheelRotation(PointerEventData eventData)
	{
		centerPoint = RectTransformUtility.WorldToScreenPoint(eventData.pressEventCamera, _wheel.position);
		wheelPrevAngle = Vector2.Angle(Vector2.up, eventData.position - centerPoint);
	}

	private void CalculateWheelRotation(PointerEventData eventData)
	{
		Vector2 pointerPos = eventData.position;

		float wheelNewAngle = Vector2.Angle(Vector2.up, pointerPos - centerPoint);
		
		if ((pointerPos - centerPoint).sqrMagnitude >= 400f)
		{
			if (pointerPos.x > centerPoint.x)
				wheelAngle += wheelNewAngle - wheelPrevAngle;

			else
				wheelAngle -= wheelNewAngle - wheelPrevAngle;
		}
		
		wheelAngle = Mathf.Clamp(wheelAngle, -maxAngle, maxAngle);
		wheelPrevAngle = wheelNewAngle;
	}

	private IEnumerator ReleaseWheel()
	{
		while (wheelAngle != 0f)
		{
			float deltaAngle = releaseSpeed * Time.deltaTime;

			if (Mathf.Abs(deltaAngle) > Mathf.Abs(wheelAngle))
				wheelAngle = 0f;

			else if (wheelAngle > 0f)
				wheelAngle -= deltaAngle;

			else
				wheelAngle += deltaAngle;


			UpdateWheelImage();

			CalculateInput();

			yield return null;
		}
	}

	private void CalculateInput()
	{
		steeringInput = wheelAngle / maxAngle;
	}

	private void UpdateWheelImage()
	{
		_wheel.localEulerAngles = new Vector3(0f, 0f, -wheelAngle);
	}
}