using UnityEngine;
using Steamworks;
using Mirror;
using Steel.Networking;

namespace Steel.Steam
{
    public class SSteamServer : MonoBehaviour
    {
        public static SSteamServer singleton;

        public SSteamSettings steamSettings;

        private void Awake()
        {
            // singleton reference
            if (singleton == null)
                singleton = this;
            else if (singleton != this)
                Destroy(this);

            // setup callbacks
            SteamServer.OnSteamServersConnected += OnSteamServersConnect;
            SteamServer.OnSteamServersDisconnected += OnSteamServersDisconnect;
            SteamServer.OnSteamServerConnectFailure += OnSteamServersConnectFail;

            SteamServerInit serverInit = new SteamServerInit()
            {
                DedicatedServer = true,
                Secure = true,
                GameDescription = steamSettings.server.serverDescription,
                ModDir = steamSettings.server.modDir,
                SteamPort = steamSettings.server.steamPort,
                VersionString = steamSettings.server.serverVersion,
                GamePort = steamSettings.server.gamePort,
                IpAddress = System.Net.IPAddress.Any,
                QueryPort = steamSettings.server.queryPort
            };

            SteamServer.Init(steamSettings.applicationId, serverInit, true);

            SteamServer.MaxPlayers = SNetworkManager.singleton.maxConnections = steamSettings.server.maxPlayerCount;
            SteamServer.ServerName = steamSettings.server.serverName;
            SteamServer.MapName = steamSettings.server.serverMap;
            SteamServer.Passworded = steamSettings.server.passwordProtected;
            SteamServer.AutomaticHeartbeats = steamSettings.server.enableHeartbeats;

            SteamServer.LogOnAnonymous();
        }

        private void OnApplicationQuit()
        {
            steamSettings.Shutdown();
        }

        #region Callbacks
        private void OnSteamServersConnect()
        {
            Debug.Log("[SteamServer] Connection to Steam's API successful!");

            // set server info
            steamSettings.server.ip = SteamServer.PublicIp;

            SNetworkManager.singleton.StartServer();
        }
        private void OnSteamServersDisconnect(Result result)
        {
            Debug.Log($"[SteamServer] Disonnected from Steam's API: {result}");
            SNetworkManager.singleton.StopServer();
        }
        private void OnSteamServersConnectFail(Result result, bool trying)
        {
            Debug.Log($"[SteamServer] Failed to connect to Steam's API: {result}");
        }
        #endregion
    }
}