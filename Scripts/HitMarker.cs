using UnityEngine;
using UnityEngine.UI;

public class HitMarker : MonoBehaviour
{
    public enum State
    {
        FadedIn,
        FadedOut
    }

    [Header("Fade Settings")]
    public State state;
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public float fadeInTime;
    
    [Header("HitMarker Size Settings")]
    public float length;

    public float thickness;

    public float gap;


    [Header("HitMarker Color")]
    [Range(0f, 255f)]
    public byte red;

    [Range(0f, 255f)]
    public byte green;

    [Range(0f, 255f)]
    public byte blue;

    [Range(0f, 255f)]
    public byte alpha;
    
    [Header("Outline Size Settings")]
    public float outlineAmount;

    [Header("Assignable")]
    public CrossHairPart[] differentParts;
    public CrossHairPart[] outlineParts;

    public static HitMarker Instance { get; private set; }

    private void Awake()
    {
        //Setting This To a Singleton
        Instance = this;
    }

    public void Update()
    {
        if (state == State.FadedOut)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * fadeOutSpeed);
        }

        if (state == State.FadedIn)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * fadeInSpeed);
        }
        
        SizeSettings();
        ColorSettings();
        OutlineSizeSettings();
    }

    public void FadeIn()
    {
        state = State.FadedIn;
        Invoke(nameof(FadeOut), fadeInTime);
    }

    public void FadeOut()
    {
        state = State.FadedOut;
    }

    private void SizeSettings()
    {
        GetRectTransform(transform).sizeDelta = new Vector2(gap, gap); //Setting The Gap

        foreach (var everyObject in differentParts)
        {
            GetRectTransform(everyObject.transform).sizeDelta = new Vector2(thickness, length); //Setting The Length And Thickness Of All The HitMarker Parts
        }
    }
    
    private void OutlineSizeSettings()
    {
        GetRectTransform(transform).sizeDelta = new Vector2(gap, gap); //Setting The Gap

        foreach (var everyObject in outlineParts)
        {
            var outlineLength = length + outlineAmount;
            var outlineThickness = thickness + outlineAmount;

            if (everyObject.type == CrossHairPart.PartType.NormalPart)
                GetRectTransform(everyObject.transform).sizeDelta = new Vector2(outlineThickness, outlineLength);
			
            if (everyObject.type == CrossHairPart.PartType.InvertedSizePart)
                GetRectTransform(everyObject.transform).sizeDelta = new Vector2(outlineLength, outlineThickness);
        }
    }

    private void ColorSettings()
    {
        foreach (var everyObject in differentParts)
        {
            GetImageComponent(everyObject.transform).color = new Color32(red, green, blue, alpha); //Set The Color Of All The Parts Of The HitMarker
        }
    }

    private static RectTransform GetRectTransform(Transform rectTransform)
    {
        return rectTransform.GetComponent<RectTransform>(); //Get The RectTransform Component
    }

    private static Image GetImageComponent(Component image)
    {
        return image.GetComponent<Image>(); //Get The Image Component
    }
}
