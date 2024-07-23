using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;
using Fusion.Sockets;

public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkPrefabRef _playerPrefab;

    private NetworkObject playerObj;
    private Vector3[] positionsRace = new[]
    {
        new Vector3(-12.4f, 0.5f, -7.06f),
        new Vector3(-6.2f, 0.5f, -7.06f),
        new Vector3(0.4f, 0.5f, -7.06f),
        new Vector3(6.8f, 0.5f, -7.06f),
        new Vector3(13f, 0.5f, -7.06f)
    };
    private Vector3[] positionsLobby = new[]
    {
        new Vector3(-12.4f, 0.5f, -1736.99f),
        new Vector3(-6.2f, 0.5f, -1736.99f),
        new Vector3(0.4f, 0.5f, -1736.99f),
        new Vector3(6.8f, 0.5f, -1736.99f),
        new Vector3(13f, 0.5f, -1736.99f)
    };

    public void SetPosition(int position, string gameName, Player player)
    {
        switch (gameName)
        {
            case "Lobby":
                player.gameObject.transform.position = positionsLobby[position];
                player.transform.rotation = Quaternion.identity;
                break;
            case "Race":
                player.transform.position = positionsRace[position];
                player.transform.rotation = Quaternion.identity;
                break;
        }
    }

    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            playerObj = runner.Spawn(_playerPrefab, null, null, player);    
            playerObj.transform.position = positionsLobby[runner.ActivePlayers.Count() - 1];
            playerObj.GetComponent<Player>().number = runner.ActivePlayers.Count() - 1;
            StartCoroutine(AddPlayer());
        }
        
        //if (runner.ActivePlayers.Count() > 1) StartCoroutine(CheckGameManager());
    }

    IEnumerator AddPlayer()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.Local.activePlayers.Add(playerObj);
    }
    IEnumerator CheckGameManager()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.Local.Countdown = !GameManager.Local.Countdown;
    }

    private CharacterInputHandler _characterInputHandler;
    
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!NetworkPlayer.Local) return;

        _characterInputHandler ??= NetworkPlayer.Local.GetComponent<CharacterInputHandler>();

        input.Set(_characterInputHandler.GetLocalInputs());
        Camera.main.GetComponentInParent<CameraFollow>().SetTarget(_characterInputHandler.transform);
    }
    
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        runner.Shutdown();
    }
    
    #region Unused callbacks

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player){ }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data){ }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress){ }

    #endregion
}
