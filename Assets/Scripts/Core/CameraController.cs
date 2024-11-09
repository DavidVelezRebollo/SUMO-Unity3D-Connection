using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    
    [SerializeField] private Vector3 InitialPosition;
    [SerializeField] private float MovementSpeed = 10f;
    [SerializeField] private float LookSensitivity = 3f;
    [SerializeField] private float ZoomSensitivity = 10f;

    private TrafficSimulator _trafficSimulator;
    private Transform _transform;
    private ExitVehicleButton _exitButton;
    private bool _looking;
    private bool _lockCamera;
    private bool _cameraOnVehicle;
    private float _rotX;

    private void Awake()
    {
        if (!Instance) Instance = this;
        
        _transform = transform;
        _transform.position = InitialPosition;
    }

    private void Start()
    {
        _trafficSimulator = TrafficSimulator.Instance;
        _exitButton = FindObjectOfType<ExitVehicleButton>();
    }

    private void Update()
    {
        if (!_trafficSimulator.ServerOn()) return;
        
        HandleKeyPress();
    }

    public void LockCamera(bool locked) => _lockCamera = locked;

    public bool IsOnVehicle() => _cameraOnVehicle;

    private void HandleKeyPress()
    {
        if (!_lockCamera)
        {
            HandleMovement();
            HandleSprint();
            HandleZoom();
        }
        HandleMouse();
        
        if (Input.GetKeyDown(KeyCode.Mouse1)) ToggleLooking(true);
        else if (Input.GetKeyUp(KeyCode.Mouse1)) ToggleLooking(false);
    }

    private void HandleMovement()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            _transform.position += MovementSpeed * Time.deltaTime * -_transform.right;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            _transform.position += MovementSpeed * Time.deltaTime * _transform.right;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            _transform.position += MovementSpeed * Time.deltaTime * _transform.forward;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            _transform.position += MovementSpeed * Time.deltaTime * -_transform.forward;
    }

    private void HandleSprint()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) MovementSpeed *= 2;
        if (Input.GetKeyUp(KeyCode.LeftShift)) MovementSpeed /= 2;
    }

    private void HandleZoom()
    {
        float zoom = Input.GetAxis("Mouse ScrollWheel");
        if (zoom != 0) _transform.position += zoom * ZoomSensitivity * -_transform.up;
    }

    private void HandleMouse()
    {
        if (!_looking) return;
        
        float horizontal = Input.GetAxis("Mouse X") * LookSensitivity;
        float vertical = Input.GetAxis("Mouse Y") * LookSensitivity;

        _transform.Rotate(Vector3.up * horizontal, Space.World);
        var rotY = _transform.localEulerAngles.y;
        _rotX += vertical;

        _transform.localEulerAngles = new Vector3(-_rotX, rotY);
    }

    private void ToggleLooking(bool looking)
    {
        _looking = looking;
        Cursor.visible = !looking;
        Cursor.lockState = looking ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public void ResetCamera()
    {
        _transform.SetParent(null);
        _transform.position = InitialPosition;
        _transform.localEulerAngles = Vector3.zero;
        _lockCamera = false;
        _cameraOnVehicle = false;
        _exitButton.Hide();
    }

    public void SetOnVehicle(Transform cameraSpot)
    {
        transform.parent = cameraSpot;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _cameraOnVehicle = true;
        LockCamera(true);
        _exitButton.Show();
    }
}
