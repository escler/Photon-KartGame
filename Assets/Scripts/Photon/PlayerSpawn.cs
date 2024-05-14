using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    public Transform[] _spawnsPos = new Transform[2];
    
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            Runner.Spawn(_playerPrefab, _spawnsPos[Runner.ActivePlayers.Count()-1].position, Quaternion.identity);
        }
    }
}
