using UnityEngine;
using Steamworks;
using Steamworks.Data;
using Mirror;
using Steel.Networking;

namespace Steel.Steam
{
    public class SSteamClient : MonoBehaviour
    {
        public static SSteamClient singleton;
        public SSteamSettings steamSettings;

        private SNetworkManager networkManager;

        private void Awake()
        {
            // singleton reference
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(this);

            networkManager = GetComponent<SNetworkManager>();

            // setup callbacks
            SteamUser.OnSteamServersConnected += OnSteamServersConnect;
            SteamUser.OnSteamServersDisconnected += OnSteamServersDisconnect;
            SteamUser.OnSteamServerConnectFailure += OnSteamServersConnectFail;
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;

            // initialize steam
            SteamClient.Init(steamSettings.applicationId);
        }

        public void ConnectLobby(SteamId lobbyId)
        {
            if (!SteamClient.IsValid) return;
            Debug.Log($"[C] Attempting to join lobby...");

            SteamMatchmaking.JoinLobbyAsync(lobbyId);
        }

        public void ConnectToServer(string ipAddress)
        {
            networkManager.networkAddress = ipAddress;
            networkManager.StartClient();
        }

        public void HostLobby()
        {
            if (!SteamClient.IsValid) return;
            Debug.Log("[C] Creating Lobby...");

            SteamMatchmaking.CreateLobbyAsync(steamSettings.server.maxPlayerCount);
        }

        #region Callbacks
        private void OnSteamServersConnect()
        {
            Debug.Log("[C] Successfully connected to steam servers!");
        }
        private void OnSteamServersDisconnect()
        {
            Debug.Log("[C] Disconnected from steam servers!");
        }
        private void OnSteamServersConnectFail()
        {
            Debug.Log("[C] Failed to connect to steam servers!");
        }
        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            Debug.Log($"[C] Lobby Created: {result}");
            if (result != Result.OK) return;

            // start networking host
            networkManager.StartHost();
            lobby.SetJoinable(true);
            lobby.SetGameServer(lobby.Id);
            lobby.SetPublic();

            // set connection info for lobby
            lobby.SetData("HostAddress", SteamClient.SteamId.ToString());
        }
        private void OnLobbyEntered(Lobby lobby)
        {
            Debug.Log($"[C] Entered Game Lobby, Starting Client...");

            // if hosting server, dont join server
            if (NetworkServer.active) return;

            // get host address
            string hostAddress = lobby.GetData("HostAddress");

            // connect as client
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
        }
        private void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log($"[C] Received Game Invite from: {friend.Name}");
        }
        #endregion
    }
}