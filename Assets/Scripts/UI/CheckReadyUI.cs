using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class CheckReadyUI : NetworkBehaviour
{
    public static CheckReadyUI Local { get; private set; }
    [SerializeField] private Player localPlayer;
    
    [SerializeField] private GameObject uiText;
    [SerializeField] private GameObject uiQuitLobby;
    [SerializeField] private GameObject uiLobby;

    [SerializeField] private TextMeshProUGUI _tmp;
    private ChangeDetector _changeDetector;
    [Networked] public NetworkBool PlayersChange { get; set; }
    [Networked] public NetworkBool InRacing { get; set; }
    
    public override void Spawned()
    {
        StartCoroutine(GetLocalPlayer());
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        Local = this;
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(PlayersChange):
                    ChangeText();
                    break;
                case nameof(InRacing):
                    _tmp.enabled = !InRacing;
                    break;
            }
        }
    }

    private void ChangeText()
    {
        _tmp.text = "Players Ready:\n " + GameManager.Local.countReady + "/" + GameManager.Local.activePlayers.Count;
    }
    
    public void ChangeValue()
    {
        if (GameManager.Local.win)
        {
            uiLobby.SetActive(true);
        }
        
        if (GameManager.Local.raceIsStarted)
        {
            uiText.SetActive(false);
            return;
        }
        uiText.SetActive(!localPlayer.readyCheck);
        uiQuitLobby.SetActive(!localPlayer.readyCheck);
    }

    public void HideReturnLobby()
    {
        uiLobby.SetActive(false);
    }
    public void HideExit()
    {
        uiText.SetActive(false);
    }

    IEnumerator GetLocalPlayer()
    {
        yield return new WaitForSeconds(.1f);
        localPlayer = NetworkPlayer.Local.GetComponent<Player>();
        localPlayer.OnReadyCheckChange += ChangeValue;
        ChangeValue();
        ChangeText();
    }
}
