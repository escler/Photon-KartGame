using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

public class CountDownUI : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI _tmp;
    [Networked] public float timer { get; set; }
    private float playerCount;

    public void RPCStartTimer()
    {
        if (!HasStateAuthority) return;
        _tmp.text = timer.ToString();
        StartCoroutine("Count");
    }

    [Rpc]
    public void RPCUpdateTimer()
    {
        _tmp.text = timer.ToString();
    }
    
    [Rpc]
    public void RPCDisableTimer()
    {
        gameObject.SetActive(false);
    }
    
    IEnumerator Count()
    {
        while (timer > 0)
        {
            timer--;
            RPCUpdateTimer();
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.RPCMoveObjects();
        RPCDisableTimer();
    }
}
