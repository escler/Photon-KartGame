using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Local { get; private set; }
    [Networked] public NetworkBool win { get; set; }
    [Networked] public NetworkBool NewRaceBegin { get; set; }
    [Networked] public NetworkBool StartRace { get; set; }
    public int lapsForWin;
    public GameObject winGo, loseGo, countDownGo, lapsGo;

    [Networked, Capacity(5)]public NetworkLinkedList<NetworkObject> activePlayers { get; }
    
    private ChangeDetector _changeDetector;
    private NetworkPlayer _winner;
    [Networked] public int countReady { get; set; }
    public bool ScreenShowed { get; set; }
    [Networked] public NetworkBool raceIsStarted { get; set; }
    [Networked] public NetworkBool ReturnToLobby { get; set; }

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
                case nameof(NewRaceBegin):
                {
                    ResetValues();
                    ShowTimeCount();
                    break;
                }
                case nameof(StartRace):
                {
                    PlayerMoving();
                    ShowLaps();
                    UncheckedReadys();
                    break;
                }
                case nameof(ReturnToLobby):
                    ReturnPlayersToLobby();
                    break;
            }
        }
    }

    private void UncheckedReadys()
    {
        foreach (var player in activePlayers)
        {
            player.GetComponent<Player>().SetReadyCheck(false);
        }

        countReady = 0;

        CheckReadyUI.Local.PlayersChange = !CheckReadyUI.Local.PlayersChange;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPCWinChecker(int laps, NetworkPlayer player)
    {
        if (laps < lapsForWin) return;

        raceIsStarted = false;
        win = true;
        StartRace = !StartRace;
        _winner = player;
        CheckReadyUI.Local.InRacing = false;
        ShowScreen(_winner);
    }

    public void ReturnPlayersToLobby()
    {
        if (!win) return;
        if (HasStateAuthority) Runner.UnloadScene(SceneRef.FromIndex(2));
        StartCoroutine(GoToLobby());
    }

    public void CheckReadyState()
    {
        if (!HasStateAuthority) return;
        countReady = 0;
        foreach (var player in activePlayers)
        {
            if(!player.GetComponent<Player>().readyCheck) continue;
            countReady++;
        }

        CheckReadyUI.Local.PlayersChange = !CheckReadyUI.Local.PlayersChange;

        if (countReady == activePlayers.Count && activePlayers.Count > 1 && !raceIsStarted)
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
            p.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    
    private void ShowScreen(NetworkPlayer player)
    {
        if (ScreenShowed) return;
        if (player.HasInputAuthority) Win();
        else Lose();
    }
    private void Win()
    {
        if (winGo == null) winGo = FindObjectOfType<WinText>().gameObject;
        lapsGo.GetComponent<LapsCountUI>().FinishRace();
        winGo.GetComponent<WinText>().ShowScreen();
        ScreenShowed = true;
    }

    private void Lose()
    {
        if (loseGo == null) loseGo = FindObjectOfType<LoseText>().gameObject;
        lapsGo.GetComponent<LapsCountUI>().FinishRace();
        loseGo.GetComponent<LoseText>().ShowScreen();
        ScreenShowed = true;
    }

    private void ShowTimeCount()
    {
        CheckReadyUI.Local.InRacing = true;
        if (countDownGo == null) countDownGo = FindObjectOfType<CountDownUI>().gameObject;
        countDownGo.SetActive(true);
        countDownGo.GetComponent<CountDownUI>().StartTime();
    }

    void ResetValues()
    {
        raceIsStarted = true;
        win = false;
        ScreenShowed = false;
        winGo = FindObjectOfType<WinText>().gameObject;
        loseGo = FindObjectOfType<LoseText>().gameObject;
        CheckReadyUI.Local.HideReturnLobby();
        CheckReadyUI.Local.HideExit();
    }
    
    void ShowLaps()
    {
        if (lapsGo == null) lapsGo = FindObjectOfType<LapsCountUI>().gameObject;
        lapsGo.GetComponent<LapsCountUI>().StartRace();
    }

    IEnumerator GoToLobby()
    {
        FindObjectOfType<LoadingScreenUI>().ShowScreen();
        yield return new WaitForSeconds(.2f);
        foreach (var player in activePlayers)
        {
            var p = player.GetComponent<Player>();
            if (p == null) continue;
            p.CanMove = false;
            p.GetComponent<Rigidbody>().isKinematic = true;
            p.RPCMoveToLobby();
        }
        SoundManager.Instance.ChangeToLobbyMusic();
        CheckReadyUI.Local.PlayersChange = !CheckReadyUI.Local.PlayersChange;
        CheckReadyUI.Local.HideReturnLobby();

        yield return new WaitForSeconds(5f);
        FindObjectOfType<LoadingScreenUI>().HideScreen();
        PlayerMoving();
    }

}
