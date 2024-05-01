using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.Serialization;

public class Player : NetworkBehaviour
{
    [Header("Stats")]
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _acceleration;
    [SerializeField] private float _deceleration;
    [SerializeField] private float  _reverseMaxSpeed;
    [SerializeField] private float _spin;

    private float _appliedSpeed;

    private Rigidbody _rb;

    private float _xAxi, _yAxi;
    private bool _moving;
    
    #region Networked Color Change
    
    [Networked, OnChangedRender(nameof(OnNetColorChanged))]
    Color NetworkedColor { get; set; }

    void OnNetColorChanged() => GetComponentInChildren<Renderer>().material.color = NetworkedColor;
    
    #endregion
    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            NetworkedColor = GetComponentInChildren<Renderer>().material.color;
            Camera.main.GetComponentInParent<CameraFollow>()?.SetTarget(transform);
            _rb = GetComponent<Rigidbody>();
        }
        else
        {
            SynchronizeProperties();
        }
    }

    void SynchronizeProperties()
    {
        OnNetColorChanged();
    }
    
    void Update()
    {
        if (!HasStateAuthority) return;
        
        _xAxi = Input.GetAxis("Vertical");
        _moving = _rb.velocity != Vector3.zero;
        _yAxi = Input.GetAxis("Horizontal");
    }

    
    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        Move();
        Rotate();
    }

    private void Move()
    {
        if (_xAxi > 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, _maxSpeed, _acceleration * Runner.DeltaTime);
        else if (_xAxi < 0) _appliedSpeed = Mathf.Lerp(_appliedSpeed, -_reverseMaxSpeed, _acceleration * Runner.DeltaTime);
        else _appliedSpeed = Mathf.Lerp(_appliedSpeed, 0, _deceleration * Runner.DeltaTime);
        
        var vel = (_rb.rotation * Vector3.forward) * _appliedSpeed;
        vel.y = _rb.velocity.y;
        _rb.velocity = vel;
    }

    private void Rotate()
    {
        if (!_moving) return;
        Quaternion rot;
        var forceToSpin = _spin * Mathf.Clamp((_maxSpeed - _appliedSpeed) / _maxSpeed,0.5f,1);
        
        if(_yAxi > 0) rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles,_rb.rotation.eulerAngles + Vector3.up * forceToSpin, Runner.DeltaTime));
        else if (_yAxi < 0) rot = Quaternion.Euler(Vector3.Lerp(_rb.rotation.eulerAngles, _rb.rotation.eulerAngles - Vector3.up * forceToSpin, Runner.DeltaTime));
        else rot = _rb.rotation;
        
        _rb.MoveRotation(rot);
    }
}
