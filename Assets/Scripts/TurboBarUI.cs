using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurboBarUI : MonoBehaviour
{
    private Slider _slider;
    private Player _localPlayer;
    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.maxValue = 100f;
        TryGetLocalPlayer();
    }

    private void OnDestroy()
    {
        _localPlayer.OnTurboChange -= UpdateSlider;
    }

    private void TryGetLocalPlayer()
    {
        if (LocalPlayerReference.Instance == null)
        {
            Invoke("TryGetLocalPlayer",1f);
            return;
        }
        
        _localPlayer = LocalPlayerReference.Instance.GetComponent<Player>();
        _localPlayer.OnTurboChange += UpdateSlider;
        UpdateSlider();
    }

    public void UpdateSlider()
    {
        _slider.value = _localPlayer.NetworkedTurbo;
    }
    
}
