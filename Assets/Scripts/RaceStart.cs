using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class RaceStart : NetworkBehaviour
{
    public override void Spawned()
    {
        SetPosition();
        StartRace();
    }

    public void SetPosition()
    {
        foreach (var player in GameManager.Local.activePlayers)
        {
            player.GetComponent<Player>().CanMove = false;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player.GetComponent<Player>().ChangeLocation = !player.GetComponent<Player>().ChangeLocation;
        }
    }

    public void StartRace()
    {
        StartCoroutine(StartRaceI());
    }

    IEnumerator StartRaceI()
    {
        yield return new WaitForSeconds(.3f);
        GameManager.Local.Countdown = !GameManager.Local.Countdown;
    }
}
