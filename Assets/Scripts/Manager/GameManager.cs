using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public GameObject winGO, loseGO;
    public float lapsForWin;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPCWinChecker(int laps, PlayerRef playerRef)
    {
        if (laps < lapsForWin) return;
        if (playerRef == Runner.LocalPlayer)
        {
            Win();
        }
        else
        {
            Lose();
        }

        enabled = false;
    }

    private void Win()
    {
        winGO.SetActive(true);
    }

    private void Lose()
    {
        loseGO.SetActive(true);
    }
}
