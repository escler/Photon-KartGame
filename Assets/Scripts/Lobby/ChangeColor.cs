using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    [SerializeField] private GameObject _UIText;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            if(NetworkPlayer.Local.GetBehaviour<Player>() == player) _UIText.SetActive(true);
            player.CanChangeColor = true;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out Player player))
        {
            player.CanChangeColor = false;
            if(NetworkPlayer.Local.GetBehaviour<Player>() == player) _UIText.SetActive(false);
        }
    }
}
