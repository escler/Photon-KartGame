using System;
using System.Collections;
using UnityEngine;
using Fusion;
using System.Linq;



public class Player : NetworkBehaviour
{
    [Header("Stats")] public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public float steer;
    public float reverserMaxSpeed;
    public float turboMaxSpeed;
    public float turboAcceleration;
    public float turboDiscountPerSecond;

    private float _maxSpeed;
    private float _acceleration;
    private float _appliedSpeed;
    
    public bool readyCheck;

    [Networked] public int MapZone { get; set; }
    
    private Rigidbody _rb;

    public Material[] carColors = new Material[5];

    [Networked] public int number { get; set; }

    [Networked]
    public NetworkBool CanMove
    {
        get { return CanMove; }
        set { CanMove = value; }
    }

    private float _xAxi, _yAxi;
    private bool _moving;
    [Networked] NetworkBool Turbo { get; set; }
    [Networked] public NetworkBool LapFinish { get; set; }
    [Networked] NetworkBool ChangeColor { get; set; }
    [Networked] private NetworkBool ChangeReady { get; set; }
    [Networked] public NetworkBool ChangeLocation { get; set; }
    [Networked] public NetworkBool AddEnergy { get; set; }

    public event Action OnTurboChange = delegate { };
    public event Action OnLapFinish = delegate { };

    [Networked] public float NetworkedTurbo { get; private set; } = 100;
    [Networked] public int NetworkedColor { get; private set; } = 0;

    public int lapsCount;
    private bool _changingColor;

    public Renderer modelRenderer;

    private Material NetworkedMaterial;

    public Transform wheelLeft, wheelRight;
    
    private ChangeDetector _changeDetector;
    private PlayerView _view;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _view = GetComponentInChildren<PlayerView>();
    }

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        StartCoroutine(NotifyGameManager());
        CanMove = true;

    }

    IEnumerator NotifyGameManager()
    {
        yield return new WaitForEndOfFrame();
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Turbo):
                {
                    _view.RpcTriggerTurboPs(Turbo);
                    RPCTurbo();
                    break;
                }
                case nameof(AddEnergy):
                {
                    RefreshEnergy();
                    break;
                }
                case nameof(LapFinish):
                {
                    UpdateLapInfo();
                    break;
                }
                case nameof(ChangeColor):
                {
                    ChangeColorCount();
                    ChangeCarColor();
                    break;
                }
                case nameof(ChangeReady):
                {
                    ReadyCheck();
                    break;
                }
                case nameof(ChangeLocation):
                {
                    MoveToRace();
                    break;
                }
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData networkInputData)) return;

        if (networkInputData.checkButton) ChangeReady = !ChangeReady;

        if (networkInputData.changeColor)
        {
            ChangeColor = !ChangeColor;
        }
        if (!CanMove)
        {
            _xAxi = 0;
            _yAxi = 0;
            _rb.velocity = Vector3.zero;
            return;
        }
        _xAxi = networkInputData.verticalInput;
        _yAxi = networkInputData.horizontalInput;
        Turbo = networkInputData.turboPressed && NetworkedTurbo > 0;
        Move();
        Rotate();
    }

    private void Move()
    {
        _maxSpeed = Turbo ? turboMaxSpeed : maxSpeed;
        _acceleration = Turbo ? turboAcceleration : acceleration;

        if (_xAxi > 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, _maxSpeed, _acceleration * Runner.DeltaTime);
        else if (_xAxi < 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, -reverserMaxSpeed, 1f * Runner.DeltaTime);
        else _appliedSpeed = Mathf.Lerp(_appliedSpeed, 0, deceleration * Runner.DeltaTime);

        var vel = (_rb.rotation * Vector3.forward) * _appliedSpeed;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }

    private void Rotate()
    {
        Quaternion rot;
        var forceToSpin = steer * Mathf.Clamp((_maxSpeed - _appliedSpeed) / _maxSpeed, 0.8f, 1);

        if (_yAxi > 0)
            rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles,
                _rb.rotation.eulerAngles + Vector3.up * forceToSpin, Runner.DeltaTime));
        else if (_yAxi < 0)
            rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles,
                _rb.rotation.eulerAngles - Vector3.up * forceToSpin, Runner.DeltaTime));
        else rot = _rb.rotation;
        _rb.MoveRotation(rot);

        wheelLeft.localEulerAngles = new Vector3(0, 45 * _yAxi, 0);
        wheelRight.localEulerAngles = new Vector3(0, 45 * _yAxi, 0);
    }

    private void RPCTurbo()
    {
        StartCoroutine(TurboCoroutine());
    }

    IEnumerator TurboCoroutine()
    {
        while (Turbo && NetworkedTurbo > 0)
        {
            if(!Turbo) StopCoroutine(TurboCoroutine());
            _xAxi = 1;
            NetworkedTurbo -= turboDiscountPerSecond * Runner.DeltaTime;
            NetworkedTurbo = Mathf.Clamp(NetworkedTurbo, 0, 100);
            OnTurboChange.Invoke();
            yield return new WaitForSeconds(Runner.DeltaTime);
        }
    }

    private void RefreshEnergy()
    {
        NetworkedTurbo = 100;
        OnTurboChange.Invoke();
    }

    private void UpdateLapInfo()
    {
        lapsCount++;
        OnLapFinish.Invoke();
        CheckWin();
    }

    private void CheckWin()
    {
        GameManager.Local.RPCWinChecker(lapsCount, GetComponent<NetworkPlayer>());
    }

    private void ReadyCheck()
    {
        readyCheck = !readyCheck;
        GameManager.Local.CheckReadyState();
    }

    private void ChangeColorCount()
    {
        NetworkedColor++;
        if (NetworkedColor >= carColors.Length) NetworkedColor = 0;
    }
    
    private void ChangeCarColor()
    {
        modelRenderer.material = carColors[NetworkedColor];
    }

    public void MoveToRace()
    {
        FindObjectOfType<Spawner>().SetPosition(number, "Race", this);
    }

}

