using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private SettingsPanel PanelToShow;

    private List<SettingsPanel> _panels;

    private void Awake()
    {
        _panels = FindObjectsOfType<SettingsPanel>().ToList();
    }

    public void OnButton()
    {
        foreach (SettingsPanel panel in _panels)
        {
            if (!panel.Visible()) continue;
            
            panel.Hide();
        }
        
        PanelToShow.Show();
    }
}
