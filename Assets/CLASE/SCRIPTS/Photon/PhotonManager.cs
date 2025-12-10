using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using Unity.Mathematics;
using UnityEngine.Events;

public class PhotonManager : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField] private NetworkPrefabRef prefab; // Referencia al prefab
    [SerializeField] private NetworkRunner runner; // Runner es quien se encarga de enviar y recibir informacion, es tu medio de comunicacion con el servidor
    [SerializeField] NetworkSceneManagerDefault sceneManager;
    [SerializeField] private Transform[] spawnPoint;
    [SerializeField] private GameObject panelMenu;

    [SerializeField] Dictionary<PlayerRef,NetworkObject> players = new Dictionary<PlayerRef, NetworkObject>(); // PlayerRef es el ID de nuestro jugador en la red, NetwokrObject es el prefab/objeto de nuestro jugador

    [SerializeField] UnityEvent onPlayerJoinedToGame; // Los UnityEvents son llamadas que se hacen al invocar un evento

    #region Metodos de Photon
    /// <summary>
    /// 
    /// Callback es algo que se manda a llamar automaticamente cuando otro proceso termina
    /// 
    /// Network Runner es tu instancia de la partida
    /// </summary>

    /// <summary>
    /// El siguiente paso es tener registrados que jugadores estan entrando
    /// </summary>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            int randomSpawn = UnityEngine.Random.Range(0, spawnPoint.Length);
            NetworkObject networkPlayer = runner.Spawn(prefab, spawnPoint[randomSpawn].position, spawnPoint[randomSpawn].rotation, player);
            players.Add(player, networkPlayer);

            // Registrar jugador - SOLO en el servidor
            var gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.RegistrarNuevoJugador(player);
                Debug.Log($"[PHOTON] Jugador {player.PlayerId} registrado en GameManager");
            }
        }

        panelMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (players.TryGetValue(player, out NetworkObject networkPlayer))
        {
            runner.Despawn(networkPlayer);
            players.Remove(player);
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        NetworkInputData data = new NetworkInputData()
        {
            move = InputManager.Instance.GetMoveInput(),
            look = InputManager.Instance.GetMouseDelta(),
            isRunning = InputManager.Instance.WasRunInputPressed(),
            shoot = InputManager.Instance.ShootInputPressed()
        };        

        input.Set(data);
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }



    #endregion


    /// <summary>
    /// En este metodo vamos a crear o buscar una partida. Si no existe alguna partida o lobby, entonces
    /// nosotros lo creamos y somos el host, si ya hay una partida entonces entramos y somos el cliente.
    /// 
    /// Aqui vamos a configurar, cuantos jugadores se pueden conectar a la partida como maximo, cual mapa va a ser el que va a cargar
    /// tambien si dentro de la partida puede haber cambios de escena.
    /// 
    /// SceneRef guarda que escena se va a usar
    /// NetworkSceneInfo guarda como se van a usar las escenas en mi juego
    /// Esta puede guardar la informacion de hasta 8 escenas
    /// </summary>
    private async void StartGame(GameMode mode)
    {
        runner.AddCallbacks(this);
        runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Partida_001",
            Scene = scene,
            SceneManager = sceneManager
        });
    }

    public void StartGameAsHost()
    {
        StartGame(GameMode.Host);
    }

    public void StartGameAsClient()
    {
        StartGame(GameMode.Client);
    }

}
