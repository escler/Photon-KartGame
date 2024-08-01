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


    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            playerObj = runner.Spawn(_playerPrefab, null, null, player);    
            playerObj.GetComponent<Player>().number = runner.ActivePlayers.Count() - 1;
            playerObj.transform.position = playerObj.GetComponent<Player>().SetPosition("Lobby");
            StartCoroutine(AddPlayer());
        }
    }

    IEnumerator AddPlayer()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.Local.activePlayers.Add(playerObj);
    }
    IEnumerator CheckGameManager()
    {
        yield return new WaitForSeconds(.5f);
        GameManager.Local.NewRaceBegin = !GameManager.Local.NewRaceBegin;
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
