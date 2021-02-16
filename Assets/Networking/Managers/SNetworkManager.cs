using UnityEngine;
using Mirror;
using Steamworks;

namespace Steel.Networking
{
    public class SNetworkManager : NetworkManager
    {
        public override void OnStartServer()
        {
            base.OnStopServer();

            Debug.Log($"Listening on {Steamworks.SteamServer.PublicIp}/{Steel.Steam.SSteamSettings.current.server.gamePort}");
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("client_0", UnityEngine.SceneManagement.LoadSceneMode.Additive);
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("menu_0");

            // send auth ticket to server
            AuthTicket ticket = SteamUser.GetAuthSessionTicket();

            JoinPacket packet = new JoinPacket()
            {
                steamId = SteamClient.SteamId,
                authTicket = ticket.Data
            };

            NetworkClient.Send(packet);

            Debug.Log("[Client] Sent Join Packet to server!");
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientDisconnect(conn);

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync("client_0");
            UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("menu_0", UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
    }
}
