using System;
using System.Collections;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TrafficSimulator : MonoBehaviour
{
    [SerializeField] private int ConnectionPort;

    private JunctionManager _junctionManager;
    private VehicleManager _vehicleManager;
    private EdgeManager _edgeManager;
    private BuildingManager _buildingManager;
    private LaneManager _laneManager;
    private CameraController _cameraController;

    private InitialScreen _initialScreen;
    private HUD _hud;
    
    private Thread _thread;
    private TcpListener _server;
    private TcpClient _client;
    private NetworkStream _nwStream;

    private int _step;
    private int _maxStep;
    private bool _simulationStep = true;

    public Action<int, int> OnStepChange;

    private void Awake()
    {
        _junctionManager = GetComponent<JunctionManager>();
        _vehicleManager = GetComponent<VehicleManager>();
        _edgeManager = GetComponent<EdgeManager>();
        _buildingManager = GetComponent<BuildingManager>();
        _laneManager = GetComponent<LaneManager>();
        _cameraController = FindObjectOfType<CameraController>();

        _initialScreen = FindObjectOfType<InitialScreen>();
        _hud = FindObjectOfType<HUD>();
    }

    public void StartThread()
    {
        ThreadStart ts = GetData;
        _thread = new Thread(ts);
        _thread.Start();
    }
    
    private void GetData()
    {
        _server = new TcpListener(IPAddress.Any, ConnectionPort);
        _server.Start();
        
        while (true)
        {
            try
            {
                try
                {
                    if (_server.Pending())
                    {
                        _client = _server.AcceptTcpClient();
                    }

                    Connection();
                }
                catch (SocketException e)
                {
                    print(e);
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }
        }
        
        _client?.Close();
    }

    private void Connection()
    {
        if (_client == null) return;
        try
        {
            _nwStream = _client.GetStream();
            byte[] buffer = new byte[_client.ReceiveBufferSize];
            int bytesRead = _nwStream.Read(buffer, 0, _client.ReceiveBufferSize);
            string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            string[] format = dataReceived.Split('\n');

            foreach (string s in format)
            {
                if (s is null or "") continue;

                string[] data = s.Split(':');
                string type = data[0], d = data[1];

                switch (type)
                {
                    case "Junction":
                        _junctionManager.SaveJunction(d);
                        break;
                    case "Vehicle":
                        _vehicleManager.AddVehicle(d);
                        break;
                    case "Edge":
                        _edgeManager.AddEdge(d);
                        break;
                    case "Polygon":
                        _buildingManager.AddBuilding(d);
                        break;
                    case "Lane":
                        _laneManager.ManageLanes(d);
                        break;
                    case "Step":
                        ParseSteps(d);
                        break;
                }
            }
        }
        catch (Exception)
        {
            // Ignore
        }
    }

    private void Update()
    {
        if (!_simulationStep) return;

        try
        {
            byte[] response = Encoding.UTF8.GetBytes("Run\n");
            _nwStream?.Write(response, 0, response.Length);
        }
        catch (Exception)
        {
            ClearManagers();
            _client?.Close();
            _server?.Stop();
            Restore();
        }
    }

    public void ManageSimulation(bool stop)
    {
        if (_nwStream == null) return;

        _simulationStep = !stop;
    }

    public void CloseServer()
    {
        byte[] response = Encoding.UTF8.GetBytes("Close");
        _nwStream?.Write(response, 0, response.Length);

        _client?.Close();
        _server?.Stop();
        
    }

    public void Restore()
    {
        _client = null;
        _nwStream = null;
        _step = 0;
        _maxStep = 0;
        _simulationStep = true;
    }

    public bool ServerOn() => _nwStream != null;

    private void ParseSteps(string data)
    {
        string[] splitData = data.Split('*');
        _step = int.Parse(splitData[0]);
        _maxStep = int.Parse(splitData[1]);

        if (_step - 1 >= _maxStep)
        {
            ClearManagers();
            _server.Stop();
            Restore();
            return;
        }
        
        OnStepChange?.Invoke(_step, _maxStep);
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

    private void OnDisable()
    {
        if (_nwStream != null) CloseServer();
        else _server?.Stop();
    }
}
