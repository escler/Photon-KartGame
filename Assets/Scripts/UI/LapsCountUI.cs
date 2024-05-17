using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class LapsCountUI : NetworkBehaviour
{
    private Player _player;
    [SerializeField] private TextMeshProUGUI _tmp;
    private int _currentLaps;

    public void OnEnable()
    {
        _player = LocalPlayerReference.Instance.GetComponent<Player>();
        UpdateLaps();
        _player.OnLapFinish += UpdateLaps;
    }

    private void UpdateLaps()
    {
        _currentLaps = _player.LapsCount + 1;
        _tmp.text = "Laps \n" + _currentLaps + "/3";
    }

}
