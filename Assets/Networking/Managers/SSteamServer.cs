using UnityEngine;
using Steamworks;
using Mirror;
using Steel.Networking;

namespace Steel.Steam
{
    public class SSteamServer : MonoBehaviour
    {
        public SSteamSettings steamSettings;

        private NetworkManager networkManager;

        private void Awake()
        {
            networkManager = GetComponent<NetworkManager>();

            // setup callbacks
            SteamServer.OnSteamServersConnected += OnSteamServersConnect;
            SteamServer.OnSteamServersDisconnected += OnSteamServersDisconnect;
            SteamServer.OnSteamServerConnectFailure += OnSteamServersConnectFail;
            SteamServer.OnValidateAuthTicketResponse += OnAuthChange;

            // initialize client
            steamSettings.InitServer();

            if (!steamSettings.steamInitialized)
            {
                Debug.LogWarning($"STEAM_API Init Server call failed!");
                return;
            }

            // set server properties
            SteamServer.Passworded = steamSettings.server.passwordProtected;
            SteamServer.MaxPlayers = steamSettings.server.maxPlayerCount;
            SteamServer.MapName = steamSettings.server.serverMap;
            SteamServer.ServerName = steamSettings.server.serverName;
            SteamServer.AutomaticHeartbeats = steamSettings.server.enableHeartbeats;
            SteamServer.Passworded = steamSettings.server.passwordProtected;
            networkManager.maxConnections = steamSettings.server.maxPlayerCount;

            // login to steam servers
            SteamServer.LogOnAnonymous();

            Debug.Log("[SteamManager] Steam Game Server Started. Waiting for connection result");
        }

        private void OnP2PSessionRequest(NetworkConnection conn, JoinPacket packet)
        {
            Debug.Log($"[SteamServer] Received P2P Session Request: {packet.steamId}");

            SteamId id = new SteamId();
            id.Value = packet.steamId;
            
            // authenticate user
            if (!SteamServer.BeginAuthSession(packet.authTicket, id))
            {
                Debug.Log($"[SteamServer] Could not authenticate client: {id}");
            }
        }

        private void OnAuthChange(SteamId id, SteamId id2, AuthResponse response)
        {
            if (response == AuthResponse.OK)
            {
                SteamNetworking.AcceptP2PSessionWithUser(id);
                Debug.Log($"[SteamServer] Accepted P2PSession: {id}");
            } else
            {
                SteamNetworking.CloseP2PSessionWithUser(id);
                Debug.Log($"[SteamServer] Denied P2PSession: {id}");
            }
        }

        private void OnApplicationQuit()
        {
            steamSettings.Shutdown();
        }

        #region Callbacks
        private void OnSteamServersConnect()
        {
            Debug.Log("[SteamServer] Connection to Steam's API successful!");

            steamSettings.server.ip = SteamServer.PublicIp;
            NetworkServer.RegisterHandler<JoinPacket>(OnP2PSessionRequest);

            networkManager.StartServer();
        }
        private void OnSteamServersDisconnect(Result result)
        {
            Debug.Log($"[SteamServer] Disonnected from Steam's API: {result}");

            if (networkManager.isNetworkActive) networkManager.StopServer();
        }
        private void OnSteamServersConnectFail(Result result, bool trying)
        {
            Debug.Log($"[SteamServer] Failed to connect to Steam's API: {result}");

            if (networkManager.isNetworkActive) networkManager.StopServer();
        }
        #endregion
    }
}