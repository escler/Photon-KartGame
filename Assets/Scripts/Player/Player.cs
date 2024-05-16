using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;
using UnityEditor;


public class Player : NetworkBehaviour
{
    [Header("Stats")]
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
    public float steer;
    public float  reverserMaxSpeed;
    public float turboMaxSpeed;
    public float turboAcceleration;
    public float turboDiscountPerSecond;

    private float _maxSpeed;
    private float _acceleration;
    private float _appliedSpeed;

    private Rigidbody _rb;
    
    public Material[] carColors = new Material[2];

    [Networked] public int number { get; set; }
    
    [Networked]
    public bool CanMove
    {
        get { return CanMove; }
        set { CanMove = value; }
    }

    private float _xAxi, _yAxi;
    private bool _moving, _turbo;

    public event Action OnTurboChange = delegate {  };

    public float NetworkedTurbo {get; private set; } = 100;
    private int _lapsCount;

    public Renderer model;

    
    #region Networked Color Change

    private Material NetworkedMaterial;
    
    #endregion
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            RPCChangeMaterial();
            Camera.main.GetComponentInParent<CameraFollow>()?.SetTarget(transform);
            _rb = GetComponent<Rigidbody>();
            SubscribeToGameManager();
            
        }
    }

    void SynchronizeProperties()
    {
        RPCChangeMaterial();
    }
    
    void Update()
    {
        if (!HasStateAuthority || !CanMove) return;
        
        _xAxi = Input.GetAxis("Vertical");
        _moving = _rb.velocity != Vector3.zero;
        _yAxi = Input.GetAxis("Horizontal");
        _turbo = Input.GetButton("Turbo") && NetworkedTurbo > 0;
        
        if(Input.GetButton("Turbo")) RPCTurbo();

    }

    
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        Move();
        Rotate();
    }

    private void Move()
    {
        _maxSpeed = _turbo ? turboMaxSpeed : maxSpeed;
        _acceleration = _turbo ? turboAcceleration : acceleration;
        
        if (_xAxi > 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, _maxSpeed, _acceleration * Runner.DeltaTime);
        else if (_xAxi < 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, -reverserMaxSpeed, _acceleration * Runner.DeltaTime);
        else _appliedSpeed = Mathf.Lerp(_appliedSpeed, 0, deceleration * Runner.DeltaTime);
        
        var vel = (_rb.rotation * Vector3.forward) * _appliedSpeed;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }

    private void Rotate()
    {
        if (!_moving) return;
        Quaternion rot;
        var forceToSpin = steer * Mathf.Clamp((_maxSpeed - _appliedSpeed) / _maxSpeed,0.5f,1);
        
        if(_yAxi > 0) rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles,_rb.rotation.eulerAngles + Vector3.up * forceToSpin, Runner.DeltaTime));
        else if (_yAxi < 0) rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles, _rb.rotation.eulerAngles - Vector3.up * forceToSpin, Runner.DeltaTime));
        else rot = _rb.rotation;
        
        _rb.MoveRotation(rot);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPCTurbo()
    {
        if (NetworkedTurbo <= 0) return;
        NetworkedTurbo -= turboDiscountPerSecond * Time.deltaTime;
        NetworkedTurbo = Mathf.Clamp(NetworkedTurbo, 0, 100);
        OnTurboChange.Invoke();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPCAddEnergy(float amount)
    {
        NetworkedTurbo += amount;
        OnTurboChange.Invoke();
    }

    public void SendVictoryChecker()
    {
        if (!HasStateAuthority) return;

        _lapsCount++;
        GameManager.Instance.RPCWinChecker(_lapsCount, Runner.LocalPlayer);
    }

    public void SubscribeToGameManager()
    {
        if (GameManager.Instance == null)
        {
            Invoke("SubscribeToGameManager",1f);
            return;
        }
        GameManager.Instance.RPCAddToDictionary(Runner.LocalPlayer,this); 
    }

    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPCChangeMaterial()
    {
        model.material = carColors[number];
    }
}
