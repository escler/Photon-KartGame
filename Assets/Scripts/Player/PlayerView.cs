using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    public GameObject psTurbo;
    public override void Spawned()
    {
        var player = GetComponentInParent<Player>();
        player.OnTurboActive += RpcTriggerTurboPs;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    void RpcTriggerTurboPs(bool turboState)
    {
        if (Runner.ActivePlayers.Count() < 2) return;
        psTurbo.SetActive(turboState);
    }
}
