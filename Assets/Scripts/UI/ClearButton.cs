using System;
using UnityEngine;

public class ClearButton : MonoBehaviour
{
    private RouteText _text;

    private void Awake()
    {
        _text = FindObjectOfType<RouteText>();
    }

    public void OnCleanButton()
    {
        if (Utils.GetExecutableRoute() == null) return;
        
        Utils.SetExecutableRoute(null);
        _text.SetRoute(null);
    }
}
