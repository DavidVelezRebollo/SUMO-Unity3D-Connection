using UnityEngine;
using UnityEngine.UI;

public class SimulationButton : MonoBehaviour
{
    [SerializeField] private Sprite Play;
    [SerializeField] private Sprite Pause;
    
    private TrafficSimulator _trafficSimulator;
    private Image _image;
    private bool _stopped;

    private void Awake()
    {
        _trafficSimulator = FindObjectOfType<TrafficSimulator>();
        _image = GetComponent<Image>();
    }

    public void OnButtonPress()
    {
        if (!_trafficSimulator.ServerOn()) return;

        _stopped = !_stopped;
        _image.sprite = _stopped ? Play : Pause;
        _trafficSimulator.ManageSimulation(_stopped);
    }
}
