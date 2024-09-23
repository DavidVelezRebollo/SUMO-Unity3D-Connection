using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StepsInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField StepsInputField;

    private void Start()
    {
        SetInputFields(PlayerPrefs.GetInt("Steps", 1000));
    }

    public void ChangeSteps()
    {
        int parsedSteps = int.Parse(StepsInputField.text);
        if (parsedSteps <= 0) return;
        
        SetInputFields(parsedSteps);
        StepsInputField.text = "";
        PlayerPrefs.SetInt("Steps", parsedSteps);
    }

    private void SetInputFields(int steps)
    {
        TrafficSimulator.Instance.SetMaxSteps(steps);
        StepsInputField.placeholder.GetComponent<TMP_Text>().SetText(steps.ToString());
    }
}
