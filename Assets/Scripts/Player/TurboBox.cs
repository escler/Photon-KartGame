using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurboBox : NetworkBehaviour
{
    private ChangeDetector _changeDetector;
    public float amount, timeForSpawnAgain;
    [Networked] private NetworkBool BoxAdquired { get; set; }

    
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
                case nameof(BoxAdquired):
                {
                    DisableCol();
                    break;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            print("PLayer");
            player.AddEnergy = !player.AddEnergy;
            BoxAdquired = !BoxAdquired;
            SoundManager.Instance.PlayNitroCollectedSound();
        }
    }

    private void DisableCol()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine("EnableBox");
    }

    IEnumerator EnableBox()
    {
        yield return new WaitForSeconds(timeForSpawnAgain);
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
    }
}
