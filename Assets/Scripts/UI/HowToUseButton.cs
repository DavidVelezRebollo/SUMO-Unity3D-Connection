using System;
using UnityEngine;

public class HowToUseButton : MonoBehaviour
{
    private HowToUseScreen _screen;
    private InitialScreen _initialScreen;

    private void Awake()
    {
        _screen = FindObjectOfType<HowToUseScreen>();
        _initialScreen = GetComponentInParent<InitialScreen>();
    }

    public void OnButtonPress()
    {
        _initialScreen.ToggleVisibility(false);
        _screen.ToggleVisibility(true);
    }
}
