using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : NetworkBehaviour
{
    public Dictionary<PlayerRef, Player> playerList = new Dictionary<PlayerRef, Player>();
    public static GameManager Instance { get; private set; }

    public GameObject winGO, loseGO, countDownGo, lapsGO;
    public float lapsForWin;
    [Networked] public bool GameStart { get; set; }
    
    public override void Spawned()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        GameStart = false;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
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

    
    [Rpc]
    public void RPCAddToDictionary(PlayerRef playerRef, Player player)
    {
        playerList.Add(playerRef,player);
        playerList[playerRef].CanMove = false;
        if (!HasStateAuthority) return;
        RPCCheckCountPlayers();
    }
    
    [Rpc]
    private void RPCCheckCountPlayers()
    {
        if (Runner.ActivePlayers.Count() < 2) return;

        countDownGo.SetActive(true);
        RPCCountdown();

    }
    
    private void RPCCountdown()
    {
        if (!HasStateAuthority) return;
        countDownGo.GetComponent<CountDownUI>().RPCStartTimer();
    }

    [Rpc]
    public void RPCMoveObjects()
    {
        foreach (var pair in playerList)
        {
            pair.Value.CanMove = true;
        }
        lapsGO.SetActive(true);

        GameStart = true;
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
