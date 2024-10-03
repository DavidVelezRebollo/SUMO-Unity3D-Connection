using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingProgressText : MonoBehaviour
{
    private static TMP_Text _text;
    private static bool _onText;
    private static string _msg;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!_onText) return;

        _text.SetText(_msg);
        _onText = false;
    }

    public static void SetText(string msg)
    {
        _msg = msg;
        _onText = true;
    }
}
