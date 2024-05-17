using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    public GameObject psTurbo;
    private bool _turboActive;
    public override void Spawned()
    {
        var player = GetComponentInParent<Player>();
        player.OnTurboActive += TriggerTurboPs;
    }

    void TriggerTurboPs(bool turboState)
    {
        psTurbo.SetActive(turboState);
    }
}
