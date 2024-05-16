using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFinishCollider : MonoBehaviour
{
    public BoxCollider nextTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            GetComponent<BoxCollider>().enabled = false;
            nextTrigger.enabled = true;
        }
    }
}
