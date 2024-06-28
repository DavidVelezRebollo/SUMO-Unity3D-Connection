using System;
using TMPro;
using UnityEngine;

public class RouteText : MonoBehaviour
{
    private TMP_Text _text;
    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
        _text.SetText(string.IsNullOrEmpty(Utils.GetExecutableRoute()) ? "sumo-gui.exe missing" : Utils.GetExecutableRoute());
    }

    public void SetRoute(string route) => _text.SetText(string.IsNullOrEmpty(route) ? "sumo-gui.exe missing" : route);
    
}
