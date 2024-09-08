using System;
using UnityEngine;

public class StopButton : MonoBehaviour
{
    private TrafficSimulator _trafficSimulator;
    public Action OnStop;

    private void Start()
    {
        _trafficSimulator = TrafficSimulator.Instance;
    }

    public void OnButtonPress()
    {
        OnStop?.Invoke();
        
        _trafficSimulator.ClearManagers();
        _trafficSimulator.CloseServer();
        _trafficSimulator.Restore();
    }
}
