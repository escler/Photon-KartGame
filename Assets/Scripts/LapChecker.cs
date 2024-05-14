using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapChecker : MonoBehaviour
{
    private int _lapsCount;
    private Player _localPlayer;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            if (!GetComponent<Player>().HasStateAuthority) return;
            _lapsCount++;
            GameManager.Instance.RPCWinChecker(_lapsCount, GetComponent<Player>().Runner.LocalPlayer);
            
        }
    }
}
