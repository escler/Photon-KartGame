using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    public GameObject psTurbo;

    public void RpcTriggerTurboPs(bool turboState)
    {
        psTurbo.SetActive(turboState);
    }
}
