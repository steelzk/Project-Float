using UnityEngine;
using Steamworks;
using Mirror;
using Steel.Networking;

namespace Steel.Steam
{
    public class SSteamClient : MonoBehaviour
    {
        public SSteamSettings steamSettings;

        private void Awake()
        {
            // setup callbacks
            SteamUser.OnSteamServersConnected += OnSteamServersConnect;
            SteamUser.OnSteamServersDisconnected += OnSteamServersDisconnect;
            SteamUser.OnSteamServerConnectFailure += OnSteamServersConnectFail;

            // initialize client
            steamSettings.InitClient();
        }

        public void ConnectToServer(string ipAddress)
        {
            if (!steamSettings.steamInitialized) return;

            NetworkClient.Connect(ipAddress);
        }

        private void OnApplicationQuit()
        {
            steamSettings.Shutdown();
        }

        #region Callbacks
        private void OnSteamServersConnect()
        {
            Debug.Log("Client successfully connected to steam servers!");
        }
        private void OnSteamServersDisconnect()
        {
            Debug.Log("Client disconnected from steam servers!");
        }
        private void OnSteamServersConnectFail()
        {
            Debug.Log("Client failed to connect to steam servers!");
        }
        #endregion
    }
}