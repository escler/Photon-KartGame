using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TurboBox : NetworkBehaviour
{
    public float amount, timeForSpawnAgain;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            player.RPCAddEnergy(amount);
            transform.GetChild(0).gameObject.SetActive(false);
            GetComponent<BoxCollider>().enabled = false;
            StartCoroutine("EnableBox");
        }
    }

    IEnumerator EnableBox()
    {
        yield return new WaitForSeconds(timeForSpawnAgain);
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;


    }
}
