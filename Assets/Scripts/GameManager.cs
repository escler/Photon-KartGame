using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool raceIsStarted;

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
                    raceIsStarted = true;
                    break;
                }
                case nameof(StartRace):
                {
                    PlayerMoving();
                    ShowLaps();
                    UncheckedReadys();
                    break;
                }
            }
        }
    }

    private void UncheckedReadys()
    {
        foreach (var player in activePlayers)
        {
            player.GetComponent<Player>().ChangeReady = false;
        }
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPCWinChecker(int laps, NetworkPlayer player)
    {
        if (laps < lapsForWin) return;

        raceIsStarted = false;
        win = true;
        StartRace = !StartRace;
        _winner = player;
        ShowScreen(_winner);
    }

    public void CheckReadyState()
    {
        if (!HasStateAuthority) return;
        int countReady = 0;
        foreach (var player in activePlayers)
        {
            if(!player.GetComponent<Player>().readyCheck) break;
            countReady++;
        }

        if (countReady == activePlayers.Count && activePlayers.Count > 0 && !raceIsStarted)
        {
            Runner.UnloadScene(SceneRef.FromIndex(2));
            Runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Additive);

            foreach (var player in activePlayers)
            {
                player.GetComponent<Player>().readyCheck = false;
                player.GetComponent<Player>().CanMove = player.GetComponent<Player>().CanMove = false;
            }
        }
    }

    private void PlayerMoving()
    {
        foreach (var player in activePlayers)
        {
            var p = player.GetComponent<Player>();
            if (p == null) continue;
            p.CanMove = true;
        }
    }
    
    private void ShowScreen(NetworkPlayer player)
    {
        if (_screenShowed) return;
        lapsGo.GetComponent<LapsCountUI>().FinishRace();
        if (player.HasInputAuthority) Win();
        else Lose();
    }
    private void Win()
    {
        if (winGo == null) winGo = FindObjectOfType<WinText>().gameObject;
        winGo.GetComponent<WinText>().ShowScreen();
        _screenShowed = true;
    }

    private void Lose()
    {
        if (loseGo == null) loseGo = FindObjectOfType<LoseText>().gameObject;
        loseGo.GetComponent<LoseText>().ShowScreen();;
        _screenShowed = true;
    }

    private void ShowTimeCount()
    {
        if (countDownGo == null) countDownGo = FindObjectOfType<CountDownUI>().gameObject;
        countDownGo.SetActive(true);
        countDownGo.GetComponent<CountDownUI>().StartTime();
    }

    void ShowLaps()
    {
        if (lapsGo == null) lapsGo = FindObjectOfType<LapsCountUI>().gameObject;
        lapsGo.GetComponent<LapsCountUI>().StartRace();
    }
}
