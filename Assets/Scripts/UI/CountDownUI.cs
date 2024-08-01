using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CountDownUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmp;
    [Networked] private float timer { get; set; }
    [Networked] private NetworkBool StartTimer { get; set; }
    [Networked] private NetworkBool RefreshTimer { get; set; }
    [Networked] private NetworkBool DisableTime { get; set; }
    private ChangeDetector _changeDetector;

    
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(StartTimer):
                    ResetTimer();
                    break;
                case nameof(RefreshTimer):
                    UpdateTimer();
                    break;
                case nameof(DisableTime):
                    DisableTimer();
                    break;
            }
        }
    }
    
    public void StartTime()
    {
        _tmp.enabled = true;
        StartTimer = !StartTimer;
    }

    private void ResetTimer()
    {
        timer = 10;
        _tmp.text = timer.ToString();
        if (!HasStateAuthority) return;
        
        StopCoroutine("Count");
        StartCoroutine("Count");
    }

    private void UpdateTimer()
    {
        _tmp.text = timer.ToString();
        if (timer <= 5)
        {
            SoundManager.Instance.ChangeToRaceMusic();  
            FindObjectOfType<LoadingScreenUI>().HideScreen();
        }
        
        if(timer == 3) SoundManager.Instance.PlayCountDownSound();
    }
    
    private void DisableTimer()
    {
        _tmp.enabled = false;
        StopCoroutine("Count");
    }

    void OnDisable()
    {
        StopCoroutine("Count");
    }
    
    IEnumerator Count()
    {
        while (timer > 1)
        {
            timer--;
            RefreshTimer = !RefreshTimer;
            yield return new WaitForSeconds(1f);
        }

        GameManager.Local.StartRace = !GameManager.Local.StartRace;
        DisableTime = !DisableTime;
    }
}
