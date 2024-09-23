using System.Collections;
using System.Threading;
using UnityEngine;
using SimpleFileBrowser;

public class LoadButton : MonoBehaviour
{
    private InitialScreen _initialScreen;
    private TrafficSimulator _trafficSimulator;
    private WarningScreen _warningScreen;
    private HUD _hud;
    
    private Thread _thread;
    private string _sumoGUI;
    private string _sumoCfg;

    private void Awake()
    {
        _initialScreen = GetComponentInParent<InitialScreen>();
        _warningScreen = FindObjectOfType<WarningScreen>();
        
        _hud = FindObjectOfType<HUD>();
    }

    private void Start()
    {
        _trafficSimulator = TrafficSimulator.Instance;
    }

    public void OnButtonPress()
    {
        StartCoroutine(LoadFiles());
    }
    
    private IEnumerator LoadFiles()
    {
        if (string.IsNullOrEmpty(Utils.GetExecutableRoute()))
        {
            WarningScreen.Instance.OnError("Missing SUMO-GUI executable file");
            yield break;
        }
        
        _initialScreen.ToggleVisibility(false);
        
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Sumo Config File", ".sumocfg"));
        FileBrowser.SetDefaultFilter(".sumocfg");

        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files);

        if (FileBrowser.Success)
        {
            _sumoCfg = FileBrowser.Result[0];
        }
        else
        {
            Restore();
            yield break;
        }

        _sumoGUI = Utils.GetExecutableRoute();
        _sumoCfg = _sumoCfg.Replace('\\', '/');
        
        _hud.ToggleVisibility(true);
        StartCoroutine(_trafficSimulator.StartThread(_sumoGUI, _sumoCfg));
    }

    private void Restore()
    {
        _sumoGUI = null;
        _sumoCfg = null;
        _initialScreen.ToggleVisibility(true);
    }
}
