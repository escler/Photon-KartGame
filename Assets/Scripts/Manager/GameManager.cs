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

    public GameObject winGo, loseGo, countDownGo, lapsGo;
    public float lapsForWin;
    [Networked] public bool GameStart { get; set; }
    [Networked] public bool GameEnd { get; set; }
    
    public override void Spawned()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPCWinChecker(int laps, PlayerRef playerRef)
    {
        if (laps < lapsForWin || GameEnd) return;
        if (playerRef == Runner.LocalPlayer)
        {
            Win();
        }
        else
        {
            Lose();
        }

        enabled = false;
        GameEnd = true;
        RPCStopPlayers();
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
    public void RPCRemovePlayerFromDictionary(PlayerRef playerRef)
    {
        if (playerList.ContainsKey(playerRef)) playerList.Remove(playerRef);

        RPCCheckCountPlayers();
    }
    
    [Rpc]
    private void RPCCheckCountPlayers()
    {
        if (Runner.ActivePlayers.Count() < 2)
        {
            countDownGo.SetActive(false);
            return;
        }

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
            pair.Value.CanMovePlayer();
        }
        lapsGo.SetActive(true);

        GameStart = true;
    }
    
    [Rpc]
    public void RPCStopPlayers()
    {
        foreach (var pair in playerList)
        {
            pair.Value.StopMovePlayer();
        }
    }

    private void Win()
    {
        winGo.SetActive(true);
    }

    private void Lose()
    {
        loseGo.SetActive(true);
    }
}
