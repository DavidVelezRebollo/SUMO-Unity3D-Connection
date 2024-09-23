using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CeilingCheck : MonoBehaviour
{
    private Toggle _toggle;
    private BuildingManager _buildingManager;

    private void Awake()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
        _toggle = GetComponent<Toggle>();
        _toggle.isOn = PlayerPrefs.GetInt("Ceiling", 1) == 1;
    }

    public void OnCheck(bool ceiling)
    {
        _buildingManager.ChangeCeilingState(ceiling);
        PlayerPrefs.SetInt("Ceiling", ceiling ? 1 : 0);
    }
}
