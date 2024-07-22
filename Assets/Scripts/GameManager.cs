using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Local { get; private set; }
    [Networked] private NetworkBool win { get; set; }
    [Networked] public NetworkBool Countdown { get; set; }
    [Networked] public NetworkBool StartRace { get; set; }
    public int lapsForWin;
    public GameObject winGo, loseGo, countDownGo, lapsGo;

    public List<NetworkObject> activePlayers = new List<NetworkObject>();
    
    private ChangeDetector _changeDetector;
    private NetworkPlayer _winner;
    private bool _screenShowed;

    public override void Spawned()
    {
        Local = this;
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(Countdown):
                {
                    ShowTimeCount();
                    break;
                }
                case nameof(StartRace):
                {
                    PlayerMoving();
                    ShowLaps();
                    break;
                }
            }
        }
    }
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPCWinChecker(int laps, NetworkPlayer player)
    {
        
        if (laps < lapsForWin) return;

        win = true;
        StartRace = !StartRace;
        _winner = player;
        ShowScreen(_winner);
    }

    private void PlayerMoving()
    {
        foreach (var player in activePlayers)
        {
            var p = player.GetComponent<Player>();
            if (p == null) continue;
            p.CanMove = !p.CanMove;
        }
    }
    
    private void ShowScreen(NetworkPlayer player)
    {
        if (_screenShowed) return;
        if (player.HasInputAuthority) Win();
        else Lose();
    }
    private void Win()
    {
        winGo.SetActive(true);
        _screenShowed = true;
    }

    private void Lose()
    {
        loseGo.SetActive(true);
        _screenShowed = true;
    }

    private void ShowTimeCount()
    {
        countDownGo.SetActive(true);
        countDownGo.GetComponent<CountDownUI>().StartTime();
    }

    void ShowLaps()
    {
        lapsGo.SetActive(true);
    }
}
