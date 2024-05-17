using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TriggerFinishCollider : MonoBehaviour
{
    public BoxCollider nextTrigger;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Player player))
        {
            if (!player.GetComponent<LocalPlayerReference>().isLocal) return;
            GetComponent<BoxCollider>().enabled = false;
            nextTrigger.enabled = true;
        }
    }
}
