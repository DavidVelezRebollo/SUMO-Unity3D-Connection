using UnityEngine;

public class StopButton : MonoBehaviour
{
    private TrafficSimulator _trafficSimulator;

    private void Start()
    {
        _trafficSimulator = TrafficSimulator.Instance;
    }

    public void OnButtonPress()
    {
        _trafficSimulator.ClearManagers();
        _trafficSimulator.CloseServer();
        _trafficSimulator.Restore();
    }
}
