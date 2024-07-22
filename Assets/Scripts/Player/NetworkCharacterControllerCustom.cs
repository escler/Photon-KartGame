using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkCharacterControllerCustom : NetworkCharacterController
{
    private float _speed;
    public override void Move(Vector3 direction)
    {
        /*var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;
        var player = GetComponent<Player>();

        direction = direction.normalized;
        
        player.maxSpeed = player.Turbo ? player.turboMaxSpeed : player.maxSpeed;
        player.acceleration = player.Turbo ? player.turboAcceleration : player.acceleration;

        player._maxSpeed = player.Turbo ? player.turboMaxSpeed : maxSpeed;
        player._acceleration = player.Turbo ? player.turboAcceleration : acceleration;
        
        if (player.n.verticalInput > 0) player.appliedSpeed = Mathf.Lerp(player.appliedSpeed, player.maxSpeed, player._acceleration * Runner.DeltaTime);
        else if (player.n.verticalInput < 0) player.appliedSpeed = Mathf.Lerp(player.appliedSpeed, -player.reverserMaxSpeed, 1f * Runner.DeltaTime);
        else player.appliedSpeed = Mathf.Lerp(player.appliedSpeed, 0, player.deceleration * Runner.DeltaTime);
        
        var vel = (player.rb.rotation * Vector3.forward) * player.appliedSpeed;
        vel.y = player.rb.velocity.y;
        player.rb.velocity = vel;
        
        Quaternion rot;
        var forceToSpin = player.steer * Mathf.Clamp((player.maxSpeed - player.appliedSpeed) / player.maxSpeed,0.5f,1);
        
        if(player.n.horizontalInput > 0) rot = Quaternion.Euler(Vector3.Lerp(player.rb.rotation.eulerAngles,player.rb.rotation.eulerAngles + Vector3.up * forceToSpin, Runner.DeltaTime));
        else if (player.n.horizontalInput < 0) rot = Quaternion.Euler(Vector3.Lerp(player.rb.rotation.eulerAngles, player.rb.rotation.eulerAngles - Vector3.up * forceToSpin, Runner.DeltaTime));
        else rot = player.rb.rotation;
        player.rb.MoveRotation(rot);
        
        Velocity = player.rb.velocity;
        
        //var vel = (player.rb.rotation * -direction) * player.appliedSpeed;
        Velocity = (transform.position - previousPos) * Runner.TickRate;
        Controller.Move(moveVelocity * deltaTime);*/

    }
}
