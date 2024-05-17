using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    [SerializeField] private GameObject _playerPrefab;
    public Transform[] _spawnsPos = new Transform[5];
    private bool _gameStart;
    private int playerCount;
    
    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.ActivePlayers.Count() > 5) return;
        
        if (player == Runner.LocalPlayer)
        {
            playerCount = Runner.ActivePlayers.Count() - 1;
            var refPlayer = Runner.Spawn(_playerPrefab, Vector3.zero,Quaternion.identity);
            StartCoroutine("CheckIfGameStart");
            if (_gameStart)
            {
                Runner.Disconnect(player);
                return;
            }
            var moveToSpawn = _spawnsPos[playerCount];
            refPlayer.transform.position = moveToSpawn.position;
            refPlayer.transform.rotation = moveToSpawn.rotation;
            refPlayer.GetComponent<Player>().number = playerCount;
        }
    }
    
    IEnumerator CheckIfGameStart()
    {
        yield return new WaitForSeconds(.1f);
        _gameStart = GameManager.Instance.GameStart;
    }
}
