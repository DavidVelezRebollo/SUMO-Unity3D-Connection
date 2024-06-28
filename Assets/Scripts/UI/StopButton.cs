using UnityEngine;

public class StopButton : MonoBehaviour
{
    private TrafficSimulator _trafficSimulator;

    private void Awake()
    {
        _trafficSimulator = FindObjectOfType<TrafficSimulator>();
    }

    public void OnButtonPress()
    {
        _trafficSimulator.ClearManagers();
        _trafficSimulator.CloseServer();
        _trafficSimulator.Restore();
    }
}
