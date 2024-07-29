using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceStart : NetworkBehaviour
{
    public override void Spawned()
    {
        StartRace();
    }

    public void SetPosition()
    {
        foreach (var player in GameManager.Local.activePlayers)
        {
            player.GetComponent<Player>().MoveToRace();
        }
    }

    void FreezeVelocity()
    {
        foreach (var player in GameManager.Local.activePlayers)
        {
            player.GetComponent<Player>().CanMove = false;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public void StartRace()
    {
        StartCoroutine(StartRaceI());
    }

    IEnumerator StartRaceI()
    {
        FreezeVelocity();
        yield return new WaitForSeconds(.3f);
        SetPosition();
        yield return new WaitForSeconds(1f);
        GameManager.Local.Countdown = !GameManager.Local.Countdown;
    }
}
