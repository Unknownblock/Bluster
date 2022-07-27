using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("Information")]
    public bool isPressed;
    public bool canHold;
    public float pressTime;

    [Header("Style")] 
    public Color normalColor;
    public Color pressedColor;

    [Header("Others")] 
    public Image styleManagement;

    private void Start()
    {
        styleManagement = transform.GetComponent<Image>();
    }

    private void Update()
    {
        if (isPressed)
        {
            pressTime += Time.unscaledDeltaTime;
        }

        if (!canHold)
        {
            if (pressTime >= Time.unscaledDeltaTime * 2f)
            {
                isPressed = false;
                pressTime = 0f;
            }
        }
        
        else if (pressTime > 0f)
        {
            isPressed = true;
        }

        if (!isPressed)
        {
            styleManagement.color = normalColor;
        }
        
        else if (isPressed)
        {
            styleManagement.color = pressedColor;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (pressTime == 0f)
        {
            isPressed = true;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        pressTime = 0f;
    }
    
    private void OnDisable()
    {
        isPressed = false;
        pressTime = 0f;
    }
}
