using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using SimpleFileBrowser;
#if UNITY_EDITOR
using UnityEditor.Scripting.Python;
#endif

public class LoadButton : MonoBehaviour
{
    private InitialScreen _initialScreen;
    private TrafficSimulator _trafficSimulator;
    private WarningScreen _warningScreen;
    private HUD _hud;
    
    private Thread _thread;
    private string _sumoGUI;
    private string _sumoCfg;
    private bool _error;

    private void Awake()
    {
        _initialScreen = GetComponentInParent<InitialScreen>();
        _trafficSimulator = FindObjectOfType<TrafficSimulator>();
        _warningScreen = FindObjectOfType<WarningScreen>();
        
        _hud = FindObjectOfType<HUD>();
    }

    private void Update()
    {
        if (!_error) return;
        
        _trafficSimulator.ClearManagers();
        _trafficSimulator.CloseServer();
        _trafficSimulator.Restore();
        _error = false;
    }

    public void OnButtonPress()
    {
        StartCoroutine(LoadFiles());
    }
    
    private IEnumerator LoadFiles()
    {
        if (string.IsNullOrEmpty(Utils.GetExecutableRoute()))
        {
            _warningScreen.ToggleVisibility(true);
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
        _trafficSimulator.StartThread();

        #if UNITY_EDITOR
        ThreadStart thread = ExecutePythonScript;
        _thread = new Thread(thread);
        _thread.Start();
        #endif
    }

    #if UNITY_EDITOR
    private void ExecutePythonScript()
    {
        PythonRunner.EnsureInitialized();

        try
        {
            PythonRunner.RunString(@$"
import traci
import socket
import traci.exceptions
import time
import UnityEngine;

HOST = '127.0.0.1'
PORT = 25001
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        
sumoBinary = ""{_sumoGUI}""
FolderPath = ""{_sumoCfg}""
sumoCmd = [sumoBinary, ""-c"", FolderPath, ""--start""]

try:
    traci.start(sumoCmd)
    sock.connect((HOST, PORT))
    step = 0
           
    junctions = traci.junction.getIDList()
    edges = traci.edge.getIDList()
    lanes = traci.lane.getIDList()
    polygons = traci.polygon.getIDList()

    for j in junctions:
        junctionId = j.replace("":"", """")
        pos = traci.junction.getPosition(j)
        data = f""Junction:{{junctionId}},{{pos[0]}},{{pos[1]}}\n""
        sock.sendall(data.encode(""utf-8""))

    for e in edges:
        edgeId = e.replace("":"", """")
        fromNode = traci.edge.getFromJunction(e)
        toNode = traci.edge.getToJunction(e)
        numLanes = traci.edge.getLaneNumber(e)
        data = f""Edge:{{edgeId}},{{fromNode}},{{toNode}},{{numLanes}}\n""
        sock.sendall(data.encode(""utf-8""))

    for l in lanes:
        edgeId = traci.lane.getEdgeID(l)
        edgeId = edgeId.replace("":"", """")
        shape = traci.lane.getShape(l)
        vehicles = traci.lane.getAllowed(l)
        data = f""Lane:{{edgeId}}*{{shape}}*{{vehicles}}\n""
        sock.sendall(data.encode(""utf-8""))

    for p in polygons:
        shape = traci.polygon.getShape(p)
        polygonType = traci.polygon.getType(p)
        data = f""Polygon:{{shape}}*{{polygonType}}\n""
        sock.sendall(data.encode(""utf-8""))

    MAX_STEP = 1000

    while step < MAX_STEP:
            receive = sock.recv(1024).decode(""utf-8"")
            if receive.__contains__(""Close""):
                break
            elif receive.__contains__(""Run""):
                traci.simulationStep()
                data = f""Step:{{step}}*{{MAX_STEP}}""
                sock.sendall(data.encode(""utf-8""))
                vehicles = traci.vehicle.getIDList()
                for v in vehicles:
                    pos = traci.vehicle.getPosition3D(v)
                    speed = traci.vehicle.getSpeed(v)
                    data = f""Vehicle:{{v}},{{pos[0]}},{{pos[1]}}\n""
                    sock.sendall(data.encode(""utf-8""))
                step += 1
                time.sleep(0.1)
    traci.close()
    sock.close()
except Exception:
    traci.close()
    sock.close()
        ");
        }
        catch (Exception e)
        {
            print(e);
            _error = true;
        }
    }
    #endif

    private void Restore()
    {
        _sumoGUI = null;
        _sumoCfg = null;
        _initialScreen.ToggleVisibility(true);
    }
}
