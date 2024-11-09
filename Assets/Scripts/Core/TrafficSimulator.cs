using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CodingConnected.TraCI.NET;
using CodingConnected.TraCI.NET.Types;
using Debug = UnityEngine.Debug;

public class TrafficSimulator : MonoBehaviour
{
    [SerializeField] private int ConnectionPort;
    [SerializeField] private int MaxStep = 1000;

    public static TrafficSimulator Instance;

    private JunctionManager _junctionManager;
    private VehicleManager _vehicleManager;
    private EdgeManager _edgeManager;
    private BuildingManager _buildingManager;
    private LaneManager _laneManager;
    private CameraController _cameraController;

    private InitialScreen _initialScreen;
    private HUD _hud;
    
    private Thread _thread;
    
    private Task _task;
    private TraCIClient _traci;

    private int _step;
    private bool _simulationStep = true;
    private bool _serverOn;
    private float _speed = 1;
    private static bool m_clean;

    private bool _error;
    private bool _junctionsFinish;
    private bool _edgesFinish;
    private bool _lanesFinish;
    private bool _buildingsFinish;
    private bool _loadingFinish;

    private int _junctionsNum;
    private int _totalJunctions;
    private int _edgesNum;
    private int _totalEdges;
    private int _lanesNum;
    private int _totalLanes;
    private int _polygonsNum;
    private int _totalPolygons;

    private const string _HOST = "127.0.0.1";

    public Action OnSimulationStart;
    public Action OnSimulationFinish;
    public Action<int, int> OnStepChange;

    private void Awake()
    {
        if (Instance) Destroy(gameObject);

        Instance = this;
        
        _junctionManager = GetComponent<JunctionManager>();
        _vehicleManager = GetComponent<VehicleManager>();
        _edgeManager = GetComponent<EdgeManager>();
        _buildingManager = GetComponent<BuildingManager>();
        _laneManager = GetComponent<LaneManager>();

        _initialScreen = FindObjectOfType<InitialScreen>();
        _hud = FindObjectOfType<HUD>();
    }

    private void Start()
    {
        _cameraController = CameraController.Instance;
    }

    private void Update()
    {
        if (_loadingFinish)
        {
            OnSimulationStart?.Invoke();
            _loadingFinish = false;
            return;
        }
        
        if (!m_clean || !_error) return;

        if (_error) WarningScreen.Instance.OnError("Map loading failed. Try again.");

        ClearManagers();
        CloseServer();
        Restore();
        
        m_clean = false;
        _error = false;
    }

    public IEnumerator StartThread(string sumoGui, string sumoCfgPath)
    {
        _traci = new TraCIClient();
        Process sumoProcess = CreateSumoServer(sumoGui, sumoCfgPath, ConnectionPort);
        if (sumoProcess == null) yield break;
        
        _task = _traci.ConnectAsync(_HOST, ConnectionPort);
        yield return new WaitWhile(() => !_task.IsCompleted);

        ThreadStart ts = GetData;
        _thread = new Thread(ts);
        _thread.Start();
    }

    private static Process CreateSumoServer(string sumoGui, string sumoCfg, int port)
    {
        Process sumoProcess;
        
        try
        {
            string args = $" -c {sumoCfg} --remote-port {port} --start";
            sumoProcess = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    Arguments = args,
                    FileName = sumoGui,

                    CreateNoWindow = true,
                    UseShellExecute = false,
                    ErrorDialog = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                },
                EnableRaisingEvents = true
            };

            sumoProcess.ErrorDataReceived += SumoProcess_ErrorDataReceived;
            sumoProcess.OutputDataReceived += SumoProcess_OutputDataReceived;

            sumoProcess.Start();
            
            sumoProcess.BeginErrorReadLine();
            sumoProcess.BeginOutputReadLine();
        }
        catch (Exception e)
        {
            print($"Exception {e}");
            return null;
        }

        return sumoProcess;
    }

    private static void SumoProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        print($"SUMO stderr {e.Data}");
    }

    private static void SumoProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        print($"SUMO stdout: {e.Data}");
        m_clean = true;
    }
    
    private void GetData()
    {
        HandleJunctions();
        HandleEdges();
        HandleLanes();
        HandlePolygons();

        _loadingFinish = true;
        HandleSimulation();
    }

    private void HandleJunctions()
    {
        try
        {
            var junctions = _traci.Junction.GetIdList();
            List<Junction> junctionsList = new();
            _totalJunctions = junctions.Content.Count;

            foreach (string j in junctions.Content)
            {
                double x = _traci.Junction.GetPosition(j).Content.X;
                double z = _traci.Junction.GetPosition(j).Content.Y;

                Junction junction = new(j, new Vector3((float)x, 0, (float)z));
                junctionsList.Add(junction);
                _junctionsNum++;
                LoadingProgressText.SetText($"Loading Junctions... ({_junctionsNum} / {_totalJunctions})");
            }

            _junctionManager.SaveJunction(junctionsList);
            _junctionsFinish = true;
        }
        catch (Exception e)
        {
            _error = true;
            print(e);
        }
    }

    private void HandleEdges()
    {
        try
        {
            var edges = _traci.Edge.GetIdList();
            List<Edge> edgesList = new();
            _totalEdges = edges.Content.Count;

            foreach (string e in edges.Content)
            {
                Edge edge = new(e, _traci.Edge.GetLaneNumber(e).Content);
                edgesList.Add(edge);
                _edgesNum++;
                LoadingProgressText.SetText($"Loading Edges... ({_edgesNum} / {_totalEdges})");
            }

            _edgeManager.AddEdges(edgesList);
            _edgesFinish = true;
        }
        catch (Exception e)
        {
            _error = true;
            print(e);
        }
    }

    private void HandleLanes()
    {
        try
        {
            List<Lane> lanesList = new();
            var lanes = _traci.Lane.GetIdList();
            _totalLanes = lanes.Content.Count;

            foreach (string l in lanes.Content)
            {
                Edge edge = _edgeManager.GetEdge(_traci.Lane.GetEdgeId(l).Content);
                var vehicles = _traci.Lane.GetAllowed(l).Content;
                bool isPedestrian = !vehicles.Exists(x => x.Equals("private"));
                var s = _traci.Lane.GetShape(l).Content.Points;
                List<Vector2> shape = new();
                foreach (Position2D pos in s) shape.Add(new Vector2((float)pos.X, (float)pos.Y));

                Lane lane = new(edge, shape, isPedestrian);
                lanesList.Add(lane);
                _lanesNum++;
                LoadingProgressText.SetText($"Loading Lanes... ({_lanesNum} / {_totalLanes})");
            }

            _laneManager.AddLanes(lanesList);
            _lanesFinish = true;
        }
        catch (Exception e)
        {
            _error = true;
            print(e);
        }
    }

    private void HandlePolygons()
    {
        try
        {
            var polygons = _traci.Polygon.GetIdList();
            List<Foundation> polygonsList = new();
            _totalPolygons = polygons.Content.Count;

            foreach (string p in polygons.Content)
            {
                List<Vector2> shape = new();
                foreach (Position2D pos in _traci.Polygon.GetShape(p).Content.Points)
                    shape.Add(new Vector2((float)pos.X, (float)pos.Y));
                string type = _traci.Polygon.GetType(p).Content;

                Foundation polygon = new Foundation(shape, type);
                polygonsList.Add(polygon);
                _polygonsNum++;
                LoadingProgressText.SetText($"Loading Polygons... ({_polygonsNum} / {_totalPolygons})");
            }

            _buildingManager.AddBuildings(polygonsList);
            _buildingsFinish = true;
        }
        catch (Exception e)
        {
            _error = true;
            print(e);
        }
    }

    private void HandleVehicles(List<string> vehicles)
    {
        foreach (string v in vehicles)
        {
            Position2D pos = _traci.Vehicle.GetPosition(v).Content;
            Vector3 position = new((float) pos.X, 0, (float) pos.Y);
            
            _vehicleManager.OnVehicleReceive(v, position);
        }
    }

    private void HandleSimulation()
    {
        _serverOn = true;
        
        do
        {
            try
            {
                if (SimulationStopped()) continue;
                
                _traci.Control.SimStep();
                var vehicles = _traci.Vehicle.GetIdList().Content;
                HandleVehicles(vehicles);

                _step++;
                ParseSteps();
                Thread.Sleep(Mathf.CeilToInt(200 * _speed));
            }
            catch (NullReferenceException)
            {
                break;
            }
        } while (_step < MaxStep);

        m_clean = true;
    }

    public void ManageSimulation(bool stop)
    {
        _simulationStep = !stop;
    }

    public bool SimulationStopped() => !_simulationStep;
    public void SetSpeed(float speed) => _speed = speed;

    public void Restore()
    {
        OnSimulationFinish?.Invoke();
        
        _step = 0;
        _simulationStep = true;

        _junctionsFinish = false;
        _buildingsFinish = false;
        _edgesFinish = false;
        _lanesFinish = false;

        _junctionsNum = 0;
        _edgesNum = 0;
        _lanesNum = 0;
        _polygonsNum = 0;
    }

    public void SetMaxSteps(int steps) => MaxStep = steps; 

    public bool ServerOn() => _serverOn;

    private void ParseSteps()
    {
        if (_step - 1 >= MaxStep)
        {
            ClearManagers();
            CloseServer();
            Restore();
            return;
        }
        
        OnStepChange?.Invoke(_step, MaxStep);
    }

    public void ClearManagers()
    {
        _vehicleManager.CleanVehicles();
        _edgeManager.ClearEdges();
        _junctionManager.ClearJunctions();
        _laneManager.ClearLanes();
        _buildingManager.ClearBuildings();
        _cameraController.ResetCamera();
        
        _initialScreen.ToggleVisibility(true);
        _hud.ToggleVisibility(false);
    }

    public void CloseServer()
    {
        _traci?.Dispose();
        _serverOn = false;
    }

    private void OnDisable()
    {
       CloseServer();
    }
}
