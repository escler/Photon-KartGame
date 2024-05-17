using System.Collections;
using System.Linq;
using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private GameObject _playerPrefab;
    public Transform[] _spawnsPos = new Transform[5];
    private NetworkObject newPlayer;
    private bool _gameStart;
    private int playerCount;
    
    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.ActivePlayers.Count() > 5) return;
        
        if (player == Runner.LocalPlayer)
        {
            StartCoroutine("CheckIfGameStart");
        }
    }
    
    IEnumerator CheckIfGameStart()
    {
        yield return new WaitForSeconds(.1f);
        _gameStart = GameManager.Instance.GameStart;
        playerCount = Runner.ActivePlayers.Count() - 1;
        newPlayer = Runner.Spawn(_playerPrefab, Vector3.zero,Quaternion.identity);
        if (!_gameStart)
        {
            newPlayer.GetComponent<Player>().number = playerCount;
            var moveToSpawn = _spawnsPos[playerCount];
            newPlayer.transform.position = moveToSpawn.position;
            newPlayer.transform.rotation = moveToSpawn.rotation;
        }
        else
        {
            Runner.Despawn(newPlayer);
        }
    }

    public void PlayerLeft(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            GameManager.Instance.RPCRemovePlayerFromDictionary(player);
        }
    }
}
