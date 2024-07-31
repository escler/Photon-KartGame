using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            player.CanChangeColor = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            player.CanChangeColor = false;
        }
    }
}
