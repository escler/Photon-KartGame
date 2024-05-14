using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.SendVictoryChecker();
        }
    }
}
