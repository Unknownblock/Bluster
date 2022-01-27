using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitMarker : MonoBehaviour
{
    public enum State
    {
        FadedIn,
        FadedOut
    }

    public State state;
    public float fadeInSpeed;
    public float fadeOutSpeed;
    public float fadeInTime;

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
    }

    public void FadeIn()
    {
        state = State.FadedIn;
        print("FadeIn");
        Invoke(nameof(FadeOut), fadeInTime);
    }

    public void FadeOut()
    {
        state = State.FadedOut;
        print("FadeOut");
    }
}
