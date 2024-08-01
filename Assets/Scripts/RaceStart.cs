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
        var localPlayer = NetworkPlayer.Local.GetComponent<Player>();
        localPlayer.RPCMovingCircuit();
    }

    void FreezeVelocity()
    {
        foreach (var player in GameManager.Local.activePlayers)
        {
            player.GetComponent<Player>().CanMove = false;
            player.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    public void StartRace()
    {
        SoundManager.Instance.musicSource.Stop();
        StartCoroutine(StartRaceI());
    }

    IEnumerator StartRaceI()
    {
        FindObjectOfType<LoadingScreenUI>().ShowScreen();
        yield return new WaitForSeconds(1f);
        FreezeVelocity();
        yield return new WaitForSeconds(.3f);
        SetPosition();
        yield return new WaitForSeconds(1f);
        GameManager.Local.NewRaceBegin = !GameManager.Local.NewRaceBegin;
    }
}
