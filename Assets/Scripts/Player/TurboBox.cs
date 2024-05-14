using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurboBox : NetworkBehaviour
{
    public float amount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.GetComponent<Player>().RPCAddEnergy(amount);
            Runner.Despawn(Object);
        }
    }
}
